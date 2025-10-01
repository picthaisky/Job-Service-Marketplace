namespace JobServiceMarketplace.Domain.Entities;

public class ProviderProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Profession { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty; // JSON or comma-separated
    public string? CertificationDocuments { get; set; } // JSON array of document URLs
    public decimal HourlyRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsVerified { get; set; } = false;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
}
