namespace GoldPrice.Models;

public class PriceHistory
{
    public int Id { get; set; }
    public string MetalType { get; set; } = string.Empty; // "Gold" or "Silver"
    public decimal PricePerGram { get; set; }
    public decimal PricePerOunce { get; set; }
    public string Currency { get; set; } = "NZD";
    public DateTime FetchedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
