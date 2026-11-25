using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Infrastructure.Data;

public sealed class EfSkillSwapRepository(SkillSwapDbContext db) : ISkillSwapRepository
{
    public Task<SkillListing?> FindSkillAsync(int id, CancellationToken ct = default) => db.SkillListings.AsNoTracking().Include(x => x.Teacher).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<SkillListing>> SearchSkillsAsync(string? search, int? categoryId, CancellationToken ct = default) => await db.SkillListings.AsNoTracking().Include(x => x.Teacher).Include(x => x.Category).Where(x => x.IsActive && (categoryId == null || x.CategoryId == categoryId) && (string.IsNullOrWhiteSpace(search) || x.Title.Contains(search) || x.Description.Contains(search))).ToListAsync(ct);
    public Task<Booking?> FindBookingAsync(int id, CancellationToken ct = default) => db.Bookings.Include(x => x.SkillListing).ThenInclude(x => x.Teacher).Include(x => x.Learner).FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int userId, CancellationToken ct = default) => await db.Bookings.AsNoTracking().Include(x => x.SkillListing).ThenInclude(x => x.Teacher).Where(x => x.LearnerId == userId || x.SkillListing.TeacherId == userId).ToListAsync(ct);
    public Task<Wallet?> FindWalletAsync(int userId, CancellationToken ct = default) => db.Wallets.FirstOrDefaultAsync(x => x.UserProfileId == userId, ct);
    public Task<bool> HasScheduleConflictAsync(int userId, DateTime start, DateTime end, int? exceptBookingId = null, CancellationToken ct = default) => db.Bookings.AnyAsync(x => (x.LearnerId == userId || x.SkillListing.TeacherId == userId) && x.Status == BookingStatus.Confirmed && (exceptBookingId == null || x.Id != exceptBookingId) && x.StartTime < end && start < x.EndTime, ct);
    public Task<bool> HasReviewAsync(int bookingId, int reviewerId, CancellationToken ct = default) => db.Reviews.AnyAsync(x => x.BookingId == bookingId && x.ReviewerId == reviewerId, ct);
    public Task AddBookingAsync(Booking booking, CancellationToken ct = default) { db.Bookings.Add(booking); return Task.CompletedTask; }
    public Task AddReviewAsync(Review review, CancellationToken ct = default) { db.Reviews.Add(review); return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken ct = default) => await db.Database.BeginTransactionAsync(ct);
}
