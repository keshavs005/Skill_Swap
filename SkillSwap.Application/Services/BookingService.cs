using SkillSwap.Application.DTOs;
using SkillSwap.Application.Exceptions;
using SkillSwap.Application.Interfaces;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Application.Services;

public sealed class BookingService(ISkillSwapRepository repository) : IBookingService
{
    public async Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var skill = await repository.FindSkillAsync(request.SkillListingId, cancellationToken) ?? throw new BusinessRuleException("Skill listing was not found.");
        if (!skill.IsActive) throw new BusinessRuleException("This skill listing is inactive.");
        if (skill.TeacherId == request.LearnerId) throw new BusinessRuleException("You cannot book your own skill.");
        var start = request.StartTime.Kind == DateTimeKind.Utc ? request.StartTime : request.StartTime.ToUniversalTime();
        var end = start.AddHours(1);
        if (start <= DateTime.UtcNow) throw new BusinessRuleException("Booking time must be in the future.");
        if (await repository.HasScheduleConflictAsync(skill.TeacherId, start, end, cancellationToken: cancellationToken) || await repository.HasScheduleConflictAsync(request.LearnerId, start, end, cancellationToken: cancellationToken))
            throw new BusinessRuleException("The selected time conflicts with another confirmed session.");
        var booking = new Booking { SkillListingId = skill.Id, LearnerId = request.LearnerId, StartTime = start, EndTime = end, CreditCost = 1m };
        await repository.AddBookingAsync(booking, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        booking.SkillListing = skill;
        return ToDto(booking);
    }

    public async Task<BookingDto> AcceptAsync(int bookingId, int teacherId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBooking(bookingId, cancellationToken);
        EnsureTeacher(booking, teacherId);
        EnsureStatus(booking, BookingStatus.Pending);
        var wallet = await repository.FindWalletAsync(booking.LearnerId, cancellationToken) ?? throw new BusinessRuleException("Learner wallet was not found.");
        if (wallet.AvailableCredits < booking.CreditCost) throw new BusinessRuleException("The learner does not have enough available SkillCredits.");
        if (await repository.HasScheduleConflictAsync(booking.SkillListing.TeacherId, booking.StartTime, booking.EndTime, booking.Id, cancellationToken) || await repository.HasScheduleConflictAsync(booking.LearnerId, booking.StartTime, booking.EndTime, booking.Id, cancellationToken))
            throw new BusinessRuleException("The selected time conflicts with another confirmed session.");
        await using var transaction = await repository.BeginTransactionAsync(cancellationToken);
        wallet.AvailableCredits -= booking.CreditCost;
        wallet.ReservedCredits += booking.CreditCost;
        booking.Status = BookingStatus.Confirmed;
        await repository.SaveChangesAsync(cancellationToken);
        await transaction.DisposeAsync();
        return ToDto(booking);
    }

    public Task<BookingDto> RejectAsync(int bookingId, int teacherId, CancellationToken cancellationToken = default) => ChangeByTeacher(bookingId, teacherId, BookingStatus.Rejected, BookingStatus.Pending, cancellationToken);

    public async Task<BookingDto> CancelAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBooking(bookingId, cancellationToken);
        if (booking.LearnerId != userId && booking.SkillListing.TeacherId != userId) throw new BusinessRuleException("You cannot cancel this booking.");
        if (booking.StartTime <= DateTime.UtcNow) throw new BusinessRuleException("Only future bookings can be cancelled.");
        if (booking.Status is not (BookingStatus.Pending or BookingStatus.Confirmed)) throw new BusinessRuleException("This booking cannot be cancelled.");
        if (booking.Status == BookingStatus.Confirmed)
        {
            var wallet = await repository.FindWalletAsync(booking.LearnerId, cancellationToken) ?? throw new BusinessRuleException("Learner wallet was not found.");
            wallet.ReservedCredits -= booking.CreditCost;
            wallet.AvailableCredits += booking.CreditCost;
        }
        booking.Status = BookingStatus.Cancelled;
        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(booking);
    }

    public async Task<BookingDto> MarkDeliveredAsync(int bookingId, int teacherId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBooking(bookingId, cancellationToken);
        EnsureTeacher(booking, teacherId);
        EnsureStatus(booking, BookingStatus.Confirmed);
        booking.Status = BookingStatus.AwaitingConfirmation;
        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(booking);
    }

    public async Task<BookingDto> ConfirmCompletionAsync(int bookingId, int learnerId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBooking(bookingId, cancellationToken);
        if (booking.LearnerId != learnerId) throw new BusinessRuleException("Only the learner can confirm completion.");
        EnsureStatus(booking, BookingStatus.AwaitingConfirmation);
        var learnerWallet = await repository.FindWalletAsync(booking.LearnerId, cancellationToken) ?? throw new BusinessRuleException("Learner wallet was not found.");
        var teacherWallet = await repository.FindWalletAsync(booking.SkillListing.TeacherId, cancellationToken) ?? throw new BusinessRuleException("Teacher wallet was not found.");
        learnerWallet.ReservedCredits -= booking.CreditCost;
        teacherWallet.AvailableCredits += booking.CreditCost;
        booking.Status = BookingStatus.Completed;
        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(booking);
    }

    public async Task AddReviewAsync(ReviewRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Rating is < 1 or > 5) throw new BusinessRuleException("Rating must be between 1 and 5.");
        var booking = await GetBooking(request.BookingId, cancellationToken);
        if (booking.Status != BookingStatus.Completed || booking.LearnerId != request.ReviewerId) throw new BusinessRuleException("Reviews are only allowed for completed sessions by the learner.");
        if (await repository.HasReviewAsync(request.BookingId, request.ReviewerId, cancellationToken)) throw new BusinessRuleException("You have already reviewed this session.");
        await repository.AddReviewAsync(new Review { BookingId = request.BookingId, ReviewerId = request.ReviewerId, Rating = request.Rating, Comment = request.Comment }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<BookingDto> ChangeByTeacher(int id, int teacherId, BookingStatus target, BookingStatus required, CancellationToken ct)
    {
        var booking = await GetBooking(id, ct); EnsureTeacher(booking, teacherId); EnsureStatus(booking, required); booking.Status = target; await repository.SaveChangesAsync(ct); return ToDto(booking);
    }
    private async Task<Booking> GetBooking(int id, CancellationToken ct) => await repository.FindBookingAsync(id, ct) ?? throw new BusinessRuleException("Booking was not found.");
    private static void EnsureTeacher(Booking booking, int teacherId) { if (booking.SkillListing.TeacherId != teacherId) throw new BusinessRuleException("Only the listing teacher can perform this action."); }
    private static void EnsureStatus(Booking booking, BookingStatus status) { if (booking.Status != status) throw new BusinessRuleException($"Booking must be {status}."); }
    private static BookingDto ToDto(Booking b) => new(b.Id, b.SkillListingId, b.LearnerId, b.SkillListing.Title, b.SkillListing.Teacher.DisplayName, b.StartTime, b.EndTime, b.Status, b.CreditCost);
}
