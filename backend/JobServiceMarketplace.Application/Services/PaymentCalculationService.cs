using JobServiceMarketplace.Domain.Entities;

namespace JobServiceMarketplace.Application.Services;

/// <summary>
/// Service for calculating commission and withholding tax
/// </summary>
public class PaymentCalculationService
{
    private const decimal COMMISSION_RATE = 0.10m; // 10%
    private const decimal WITHHOLDING_TAX_RATE = 0.03m; // 3%

    /// <summary>
    /// Calculate payment breakdown including commission and withholding tax
    /// </summary>
    public PaymentCalculation CalculatePayment(decimal grossAmount)
    {
        var commissionAmount = grossAmount * COMMISSION_RATE;
        var withholdingTaxAmount = grossAmount * WITHHOLDING_TAX_RATE;
        var netAmount = grossAmount - commissionAmount - withholdingTaxAmount;

        return new PaymentCalculation
        {
            GrossAmount = grossAmount,
            CommissionAmount = commissionAmount,
            WithholdingTaxAmount = withholdingTaxAmount,
            NetAmount = netAmount
        };
    }

    /// <summary>
    /// Create payment transactions for a booking
    /// </summary>
    public List<Transaction> CreatePaymentTransactions(Payment payment)
    {
        var transactions = new List<Transaction>
        {
            new Transaction
            {
                PaymentId = payment.Id,
                Type = TransactionType.Payment,
                Amount = payment.Amount,
                Description = "Payment received from client",
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                PaymentId = payment.Id,
                Type = TransactionType.Commission,
                Amount = payment.CommissionAmount,
                Description = $"Platform commission ({COMMISSION_RATE * 100}%)",
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                PaymentId = payment.Id,
                Type = TransactionType.WithholdingTax,
                Amount = payment.WithholdingTaxAmount,
                Description = $"Withholding tax {WITHHOLDING_TAX_RATE * 100}% (ภงด.3)",
                CreatedAt = DateTime.UtcNow
            }
        };

        return transactions;
    }

    /// <summary>
    /// Create release transaction when payment is released to provider
    /// </summary>
    public Transaction CreateReleaseTransaction(Payment payment)
    {
        return new Transaction
        {
            PaymentId = payment.Id,
            Type = TransactionType.Release,
            Amount = payment.NetAmount,
            Description = $"Payment released to provider (Net: {payment.NetAmount:N2})",
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class PaymentCalculation
{
    public decimal GrossAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal WithholdingTaxAmount { get; set; }
    public decimal NetAmount { get; set; }
}
