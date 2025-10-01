namespace JobServiceMarketplace.Application.DTOs;

public class CreateProviderProfileDto
{
    public string Profession { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public List<string>? CertificationDocuments { get; set; }
}

public class UpdateProviderProfileDto
{
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? Location { get; set; }
}

public class ProviderProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Profession { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsVerified { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
}

public class ProviderProfileDetailsDto : ProviderProfileDto
{
    public UserDto? User { get; set; }
    public List<AvailabilityDto> Availabilities { get; set; } = new();
    public List<PortfolioDto> Portfolios { get; set; } = new();
}

public class CreateAvailabilityDto
{
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class AvailabilityDto
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreatePortfolioDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class PortfolioDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
