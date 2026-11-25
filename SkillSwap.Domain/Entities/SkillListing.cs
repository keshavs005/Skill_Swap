namespace SkillSwap.Domain.Entities;

public class SkillListing
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int CategoryId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserProfile Teacher { get; set; } = null!;
    public SkillCategory Category { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
