using SkillSwap.Application.DTOs;
using SkillSwap.Application.Exceptions;
using SkillSwap.Application.Interfaces;
using SkillSwap.Application.Services;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;
using Xunit;

namespace SkillSwap.Tests;

public sealed class BookingServiceTests
{
    private static (BookingService Service, FakeRepository Repo, Booking Booking) Create()
    {
        var teacher = new UserProfile { Id = 1, DisplayName = "Teacher", Email = "teacher@test" };
        var learner = new UserProfile { Id = 2, DisplayName = "Learner", Email = "learner@test" };
        var skill = new SkillListing { Id = 1, TeacherId = 1, CategoryId = 1, Title = "Python", Description = "Python", Teacher = teacher };
        var booking = new Booking { Id = 1, SkillListingId = 1, LearnerId = 2, StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(1), SkillListing = skill };
        var repo = new FakeRepository(skill, booking, new Wallet { Id = 1, UserProfileId = 2, AvailableCredits = 3 }, new Wallet { Id = 2, UserProfileId = 1 });
        return (new BookingService(repo), repo, booking);
    }

    [Fact] public async Task ValidBookingRequestIsCreated() { var (service, _, _) = Create(); var result = await service.CreateAsync(new(1, 2, DateTime.UtcNow.AddDays(2))); Assert.Equal(BookingStatus.Pending, result.Status); }
    [Fact] public async Task SelfBookingIsRejected() { var (service, _, _) = Create(); await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(new(1, 1, DateTime.UtcNow.AddDays(2)))); }
    [Fact] public async Task InsufficientCreditsPreventsAcceptance() { var (service, repo, _) = Create(); repo.LearnerWallet!.AvailableCredits = 0; await Assert.ThrowsAsync<BusinessRuleException>(() => service.AcceptAsync(1, 1)); }
    [Fact] public async Task OverlappingConfirmedSessionIsRejected() { var (service, repo, _) = Create(); repo.Conflict = true; await Assert.ThrowsAsync<BusinessRuleException>(() => service.AcceptAsync(1, 1)); }
    [Fact] public async Task CompletionTransfersExactlyOneCredit() { var (service, repo, booking) = Create(); booking.Status = BookingStatus.AwaitingConfirmation; repo.LearnerWallet!.AvailableCredits = 2; repo.LearnerWallet.ReservedCredits = 1; await service.ConfirmCompletionAsync(1, 2); Assert.Equal(0, repo.LearnerWallet.ReservedCredits); Assert.Equal(1, repo.TeacherWallet!.AvailableCredits); Assert.Equal(BookingStatus.Completed, booking.Status); }
    [Fact] public async Task CompletingTwiceIsRejected() { var (service, _, booking) = Create(); booking.Status = BookingStatus.Completed; await Assert.ThrowsAsync<BusinessRuleException>(() => service.ConfirmCompletionAsync(1, 2)); }
    [Fact] public async Task CancellationReleasesReservedCredit() { var (service, repo, booking) = Create(); booking.Status = BookingStatus.Confirmed; repo.LearnerWallet!.AvailableCredits = 2; repo.LearnerWallet.ReservedCredits = 1; await service.CancelAsync(1, 2); Assert.Equal(3, repo.LearnerWallet.AvailableCredits); Assert.Equal(0, repo.LearnerWallet.ReservedCredits); }

    private sealed class FakeRepository(SkillListing skill, Booking booking, Wallet learnerWallet, Wallet teacherWallet) : ISkillSwapRepository
    {
        public Wallet? LearnerWallet { get; } = learnerWallet; public Wallet? TeacherWallet { get; } = teacherWallet; public bool Conflict { get; set; }
        public Task<SkillListing?> FindSkillAsync(int id, CancellationToken c = default) => Task.FromResult<SkillListing?>(skill);
        public Task<IReadOnlyList<SkillListing>> SearchSkillsAsync(string? s, int? c, CancellationToken t = default) => Task.FromResult<IReadOnlyList<SkillListing>>([]);
        public Task<Booking?> FindBookingAsync(int id, CancellationToken c = default) => Task.FromResult<Booking?>(booking);
        public Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int id, CancellationToken c = default) => Task.FromResult<IReadOnlyList<Booking>>([]);
        public Task<Wallet?> FindWalletAsync(int id, CancellationToken c = default) => Task.FromResult<Wallet?>(id == 2 ? LearnerWallet : TeacherWallet);
        public Task<bool> HasScheduleConflictAsync(int id, DateTime s, DateTime e, int? x = null, CancellationToken c = default) => Task.FromResult(Conflict);
        public Task<bool> HasReviewAsync(int b, int r, CancellationToken c = default) => Task.FromResult(false);
        public Task AddBookingAsync(Booking b, CancellationToken c = default) => Task.CompletedTask;
        public Task AddReviewAsync(Review r, CancellationToken c = default) => Task.CompletedTask;
        public Task SaveChangesAsync(CancellationToken c = default) => Task.CompletedTask;
        public Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken c = default) => Task.FromResult<IAsyncDisposable>(new NoopAsyncDisposable());
        private sealed class NoopAsyncDisposable : IAsyncDisposable { public ValueTask DisposeAsync() => ValueTask.CompletedTask; }
    }
}
