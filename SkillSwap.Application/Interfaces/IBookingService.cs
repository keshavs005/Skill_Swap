using SkillSwap.Application.DTOs;

namespace SkillSwap.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<BookingDto> AcceptAsync(int bookingId, int teacherId, CancellationToken cancellationToken = default);
    Task<BookingDto> RejectAsync(int bookingId, int teacherId, CancellationToken cancellationToken = default);
    Task<BookingDto> CancelAsync(int bookingId, int userId, CancellationToken cancellationToken = default);
    Task<BookingDto> MarkDeliveredAsync(int bookingId, int teacherId, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmCompletionAsync(int bookingId, int learnerId, CancellationToken cancellationToken = default);
    Task AddReviewAsync(ReviewRequest request, CancellationToken cancellationToken = default);
}
