namespace JobServiceMarketplace.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public int ReviewerId { get; set; } // Client giving review
    public User Reviewer { get; set; } = null!;
    
    public int RevieweeId { get; set; } // Provider receiving review
    public User Reviewee { get; set; } = null!;
    
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
