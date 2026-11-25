using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Application.Interfaces;

public interface ISkillSwapRepository
{
    Task<SkillListing?> FindSkillAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SkillListing>> SearchSkillsAsync(string? search, int? categoryId, CancellationToken cancellationToken = default);
    Task<Booking?> FindBookingAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<Wallet?> FindWalletAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> HasScheduleConflictAsync(int userId, DateTime start, DateTime end, int? exceptBookingId = null, CancellationToken cancellationToken = default);
    Task<bool> HasReviewAsync(int bookingId, int reviewerId, CancellationToken cancellationToken = default);
    Task AddBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task AddReviewAsync(Review review, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
