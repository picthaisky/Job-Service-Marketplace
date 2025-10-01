namespace JobServiceMarketplace.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ProviderProfile? ProviderProfile { get; set; }
    public ICollection<Booking> BookingsAsClient { get; set; } = new List<Booking>();
    public ICollection<Booking> BookingsAsProvider { get; set; } = new List<Booking>();
    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
}

public enum UserRole
{
    Client = 1,
    Provider = 2,
    Admin = 3
}
