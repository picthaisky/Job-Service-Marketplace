namespace JobServiceMarketplace.Domain.Entities;

public class TaxDocument
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public User Provider { get; set; } = null!;
    
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public TaxDocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TaxDocumentType
{
    PND3 = 1, // ภงด.3 หนังสือรับรองการหักภาษี ณ ที่จ่าย
    Invoice = 2,
    Receipt = 3
}
