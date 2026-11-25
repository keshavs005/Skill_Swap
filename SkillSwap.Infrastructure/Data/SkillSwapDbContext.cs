using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Data;

public sealed class SkillSwapDbContext(DbContextOptions<SkillSwapDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
    public DbSet<SkillListing> SkillListings => Set<SkillListing>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Wallet>().HasIndex(x => x.UserProfileId).IsUnique();
        modelBuilder.Entity<Review>().HasIndex(x => new { x.BookingId, x.ReviewerId }).IsUnique();
        modelBuilder.Entity<Wallet>().Property(x => x.RowVersion).IsRowVersion();
        modelBuilder.Entity<Wallet>().Property(x => x.AvailableCredits).HasPrecision(18, 2);
        modelBuilder.Entity<Wallet>().Property(x => x.ReservedCredits).HasPrecision(18, 2);
        modelBuilder.Entity<CreditTransaction>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.CreditCost).HasPrecision(18, 2);
        modelBuilder.Entity<SkillListing>().HasOne(x => x.Teacher).WithMany(x => x.SkillListings).HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SkillListing>().HasOne(x => x.Category).WithMany(x => x.SkillListings).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Booking>().HasOne(x => x.Learner).WithMany(x => x.LearnerBookings).HasForeignKey(x => x.LearnerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Booking>().HasOne(x => x.SkillListing).WithMany(x => x.Bookings).HasForeignKey(x => x.SkillListingId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CreditTransaction>().HasOne(x => x.FromWallet).WithMany(x => x.OutgoingTransactions).HasForeignKey(x => x.FromWalletId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CreditTransaction>().HasOne(x => x.ToWallet).WithMany(x => x.IncomingTransactions).HasForeignKey(x => x.ToWalletId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Review>().HasOne(x => x.Reviewer).WithMany().HasForeignKey(x => x.ReviewerId).OnDelete(DeleteBehavior.Restrict);
        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var users = Enumerable.Range(1, 6).Select(i => new UserProfile { Id = i, DisplayName = $"Student {i}", Email = $"student{i}@skillswap.demo", Bio = "A SkillSwap demo student.", CreatedAt = new DateTime(2025, 1, i, 0, 0, 0, DateTimeKind.Utc) }).ToArray();
        var categories = new[] { "Programming", "Music", "Design", "Languages", "Academic", "Wellness" }.Select((name, i) => new SkillCategory { Id = i + 1, Name = name, Description = $"{name} skills" }).ToArray();
        var listings = Enumerable.Range(1, 12).Select(i => new SkillListing { Id = i, TeacherId = (i % 6) + 1, CategoryId = (i % 6) + 1, Title = new[] { "Python fundamentals", "Guitar basics", "Photoshop studio", "French conversation", "Statistics help", "Public speaking" }[(i - 1) % 6], Description = "A friendly one-hour peer learning session.", CreatedAt = new DateTime(2025, 1, 10 + i, 0, 0, 0, DateTimeKind.Utc) }).ToArray();
        modelBuilder.Entity<UserProfile>().HasData(users);
        modelBuilder.Entity<SkillCategory>().HasData(categories);
        modelBuilder.Entity<SkillListing>().HasData(listings);
        modelBuilder.Entity<Wallet>().HasData(Enumerable.Range(1, 6).Select(i => new Wallet { Id = i, UserProfileId = i, AvailableCredits = 3m, ReservedCredits = 0m, RowVersion = new byte[] { 1, 0, 0, 0 } }));
    }
}
