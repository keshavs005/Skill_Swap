using SkillSwap.Domain.Enums;

namespace SkillSwap.Domain.Entities;

public class CreditTransaction
{
    public int Id { get; set; }
    public int? BookingId { get; set; }
    public int? FromWalletId { get; set; }
    public int? ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public CreditTransactionType TransactionType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Booking? Booking { get; set; }
    public Wallet? FromWallet { get; set; }
    public Wallet? ToWallet { get; set; }
}
