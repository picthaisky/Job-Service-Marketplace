namespace JobServiceMarketplace.Domain.Entities;

public class ProviderIncomeSummary
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public User Provider { get; set; } = null!;
    
    public int Year { get; set; }
    public decimal TotalGrossIncome { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalWithholdingTax { get; set; }
    public decimal TotalNetIncome { get; set; }
    public int TotalCompletedBookings { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
