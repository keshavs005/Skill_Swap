namespace SkillSwap.Domain.Entities;

public class SkillCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<SkillListing> SkillListings { get; set; } = new List<SkillListing>();
}
