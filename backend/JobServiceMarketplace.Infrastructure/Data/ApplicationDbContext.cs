using JobServiceMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobServiceMarketplace.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ProviderProfile> ProviderProfiles { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ProviderIncomeSummary> ProviderIncomeSummaries { get; set; }
    public DbSet<TaxDocument> TaxDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(e => e.ProviderProfile)
                .WithOne(p => p.User)
                .HasForeignKey<ProviderProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProviderProfile Configuration
        modelBuilder.Entity<ProviderProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Profession).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AverageRating).HasColumnType("decimal(3,2)");

            entity.HasMany(e => e.Availabilities)
                .WithOne(a => a.ProviderProfile)
                .HasForeignKey(a => a.ProviderProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Portfolios)
                .WithOne(p => p.ProviderProfile)
                .HasForeignKey(p => p.ProviderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Booking Configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.JobDescription).HasMaxLength(2000);
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(8,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Client)
                .WithMany(u => u.BookingsAsClient)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Provider)
                .WithMany(u => u.BookingsAsProvider)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Review)
                .WithOne(r => r.Booking)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CommissionAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.WithholdingTaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.Transactions)
                .WithOne(t => t.Payment)
                .HasForeignKey(t => t.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction Configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Review Configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(1000);

            entity.HasOne(e => e.Reviewer)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(e => e.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Reviewee)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(e => e.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProviderIncomeSummary Configuration
        modelBuilder.Entity<ProviderIncomeSummary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProviderId, e.Year }).IsUnique();
            entity.Property(e => e.TotalGrossIncome).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalCommission).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalWithholdingTax).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalNetIncome).HasColumnType("decimal(18,2)");
        });

        // TaxDocument Configuration
        modelBuilder.Entity<TaxDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DocumentUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Provider)
                .WithMany()
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Booking)
                .WithMany()
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
