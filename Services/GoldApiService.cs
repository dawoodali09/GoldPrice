using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GoldPrice.Models;
using GoldPrice.Data;

namespace GoldPrice.Services;

public interface IGoldApiService
{
    Task<PriceHistory?> FetchLatestPrice(string metalType);
}

public class GoldApiService : IGoldApiService
{
    private readonly HttpClient _httpClient;
    private readonly GoldPriceDbContext _dbContext;
    private readonly ILogger<GoldApiService> _logger;
    private const string ApiKey = "goldapi-x2asmdyfgrou-io";
    private const string BaseUrl = "https://www.goldapi.io/api";

    public GoldApiService(HttpClient httpClient, GoldPriceDbContext dbContext, ILogger<GoldApiService> logger)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PriceHistory?> FetchLatestPrice(string metalType)
    {
        try
        {
            // GoldAPI uses XAU for gold and XAG for silver
            var symbol = metalType.ToUpper() == "GOLD" ? "XAU" : "XAG";

            // Fetch price in USD (NZD not directly supported)
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/{symbol}/USD");
            request.Headers.Add("x-access-token", ApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GoldAPI returned {StatusCode}: {Content}",
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<GoldApiResponse>(content);

            if (apiResponse == null)
            {
                _logger.LogError("Failed to parse GoldAPI response");
                return null;
            }

            // Get USD to NZD exchange rate
            var nzdRate = await GetUsdToNzdRate();
            if (nzdRate == 0)
            {
                _logger.LogError("Failed to get USD to NZD exchange rate");
                return null;
            }

            // Convert prices to NZD
            var pricePerGramNzd = apiResponse.PriceGram24k * nzdRate;
            var pricePerOunceNzd = apiResponse.Price * nzdRate;

            var priceHistory = new PriceHistory
            {
                MetalType = metalType,
                PricePerOunce = pricePerOunceNzd,
                PricePerGram = pricePerGramNzd,
                Currency = "NZD",
                FetchedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.PriceHistory.Add(priceHistory);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("{MetalType} price fetched: {PricePerGram:N2} NZD/gram", metalType, pricePerGramNzd);

            return priceHistory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {MetalType} price from GoldAPI", metalType);
            return null;
        }
    }

    private async Task<decimal> GetUsdToNzdRate()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var nzdRate = json["rates"]?["NZD"]?.Value<decimal>() ?? 0;

            _logger.LogInformation("USD to NZD rate: {Rate}", nzdRate);
            return nzdRate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching USD to NZD exchange rate");
            return 0;
        }
    }

    private class GoldApiResponse
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("metal")]
        public string Metal { get; set; } = string.Empty;

        [JsonProperty("price_gram_24k")]
        public decimal PriceGram24k { get; set; }
    }
}
