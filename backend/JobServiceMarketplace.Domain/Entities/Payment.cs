namespace JobServiceMarketplace.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal WithholdingTaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }
    public string? PaymentGatewayTransactionId { get; set; }
    public string? PaymentGateway { get; set; } // Stripe, Omise, TrueMoney
    
    public DateTime? PaidAt { get; set; }
    public DateTime? ReleasedToProviderAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Held = 3, // In escrow
    Released = 4,
    Refunded = 5,
    Failed = 6
}

public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    BankTransfer = 3,
    TrueMoney = 4,
    Other = 5
}
