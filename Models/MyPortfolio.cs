namespace GoldPrice.Models;

public class MyPortfolio
{
    public int Id { get; set; }
    public string MetalType { get; set; } = string.Empty; // "Gold" or "Silver"
    public decimal QuantityGrams { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
