namespace SkillSwap.Domain.Entities;

public class UserProfile
{
    public int Id { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<SkillListing> SkillListings { get; set; } = new List<SkillListing>();
    public ICollection<Booking> LearnerBookings { get; set; } = new List<Booking>();
    public Wallet? Wallet { get; set; }
}
