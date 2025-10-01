namespace JobServiceMarketplace.Domain.Entities;

public class Portfolio
{
    public int Id { get; set; }
    public int ProviderProfileId { get; set; }
    public ProviderProfile ProviderProfile { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
