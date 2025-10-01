namespace JobServiceMarketplace.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
    
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TransactionType
{
    Payment = 1,
    Commission = 2,
    WithholdingTax = 3,
    Release = 4,
    Refund = 5
}
