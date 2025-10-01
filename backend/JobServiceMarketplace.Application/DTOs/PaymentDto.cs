namespace JobServiceMarketplace.Application.DTOs;

public class CreatePaymentDto
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public string? PaymentGateway { get; set; }
    public string? PaymentGatewayTransactionId { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal WithholdingTaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int Status { get; set; }
    public int PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? ReleasedToProviderAt { get; set; }
}

public class PaymentDetailsDto : PaymentDto
{
    public List<TransactionDto> Transactions { get; set; } = new();
}

public class TransactionDto
{
    public int Id { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public int BookingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ReviewerId { get; set; }
    public int RevieweeId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProviderIncomeSummaryDto
{
    public int ProviderId { get; set; }
    public int Year { get; set; }
    public decimal TotalGrossIncome { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalWithholdingTax { get; set; }
    public decimal TotalNetIncome { get; set; }
    public int TotalCompletedBookings { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TaxDocumentDto
{
    public int Id { get; set; }
    public int DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssuedDate { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
