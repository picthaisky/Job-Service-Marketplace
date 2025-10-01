namespace JobServiceMarketplace.Application.DTOs;

public class CreateBookingDto
{
    public int ProviderId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public DateTime ScheduledStartDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal EstimatedHours { get; set; }
}

public class BookingDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ProviderId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public DateTime ScheduledStartDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingDetailsDto : BookingDto
{
    public UserDto? Client { get; set; }
    public UserDto? Provider { get; set; }
}

public class CancelBookingDto
{
    public string CancellationReason { get; set; } = string.Empty;
}
