namespace GoldPrice.Models;

public class MetalViewModel
{
    public string MetalType { get; set; } = string.Empty;
    public decimal CurrentPricePerGram { get; set; }
    public decimal CurrentPricePerOunce { get; set; }
    public DateTime? LastUpdated { get; set; }
    public decimal MyQuantityGrams { get; set; }
    public decimal MyTotalWorth { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal GainLossAmount { get; set; }
    public decimal GainLossPercentage { get; set; }
    public List<PriceHistory> PriceHistory { get; set; } = new();
    public List<PortfolioItemViewModel> PortfolioItems { get; set; } = new();
}

public class PortfolioItemViewModel
{
    public int Id { get; set; }
    public string MetalType { get; set; } = string.Empty;
    public decimal QuantityGrams { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string? Description { get; set; }
    public decimal CurrentWorth { get; set; }
    public decimal? InvestedAmount { get; set; }
    public decimal? GainLossAmount { get; set; }
    public decimal? GainLossPercentage { get; set; }
}
