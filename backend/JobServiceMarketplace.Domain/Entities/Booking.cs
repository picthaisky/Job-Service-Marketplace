namespace JobServiceMarketplace.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public User Client { get; set; } = null!;
    
    public int ProviderId { get; set; }
    public User Provider { get; set; } = null!;
    
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public DateTime ScheduledStartDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal TotalAmount { get; set; }
    
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
}

public enum BookingStatus
{
    Pending = 1,
    Accepted = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    Disputed = 6
}
