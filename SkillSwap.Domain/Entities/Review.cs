namespace SkillSwap.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ReviewerId { get; set; }
    public int Rating { get; set; }
    public required string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Booking Booking { get; set; } = null!;
    public UserProfile Reviewer { get; set; } = null!;
}
