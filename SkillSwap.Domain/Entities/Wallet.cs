namespace SkillSwap.Domain.Entities;

public class Wallet
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public decimal AvailableCredits { get; set; }
    public decimal ReservedCredits { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public UserProfile UserProfile { get; set; } = null!;
    public ICollection<CreditTransaction> OutgoingTransactions { get; set; } = new List<CreditTransaction>();
    public ICollection<CreditTransaction> IncomingTransactions { get; set; } = new List<CreditTransaction>();
}
