using SkillSwap.Domain.Enums;

namespace SkillSwap.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int SkillListingId { get; set; }
    public int LearnerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal CreditCost { get; set; } = 1m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public SkillListing SkillListing { get; set; } = null!;
    public UserProfile Learner { get; set; } = null!;
    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
