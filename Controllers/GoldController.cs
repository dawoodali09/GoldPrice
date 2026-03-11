using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoldPrice.Data;
using GoldPrice.Models;
using GoldPrice.Services;

namespace GoldPrice.Controllers;

public class GoldController : Controller
{
    private readonly GoldPriceDbContext _dbContext;
    private readonly IGoldApiService _goldApiService;

    public GoldController(GoldPriceDbContext dbContext, IGoldApiService goldApiService)
    {
        _dbContext = dbContext;
        _goldApiService = goldApiService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = await GetMetalViewModel("Gold");
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> FetchPrice()
    {
        var result = await _goldApiService.FetchLatestPrice("Gold");
        if (result == null)
        {
            TempData["Error"] = "Failed to fetch gold price. Please try again later.";
        }
        else
        {
            TempData["Success"] = $"Gold price updated: {result.PricePerGram:N2} NZD/gram";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddHolding(decimal quantity, decimal? purchasePrice, DateTime? purchaseDate, string? description)
    {
        var holding = new MyPortfolio
        {
            MetalType = "Gold",
            QuantityGrams = quantity,
            PurchasePrice = purchasePrice,
            PurchaseDate = purchaseDate,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.MyPortfolio.Add(holding);
        await _dbContext.SaveChangesAsync();
        TempData["Success"] = "Gold holding added successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> EditHolding(int id, decimal quantity, decimal? purchasePrice, DateTime? purchaseDate, string? description)
    {
        var holding = await _dbContext.MyPortfolio.FindAsync(id);
        if (holding != null && holding.MetalType == "Gold")
        {
            holding.QuantityGrams = quantity;
            holding.PurchasePrice = purchasePrice;
            holding.PurchaseDate = purchaseDate;
            holding.Description = description;
            holding.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            TempData["Success"] = "Holding updated successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteHolding(int id)
    {
        var holding = await _dbContext.MyPortfolio.FindAsync(id);
        if (holding != null && holding.MetalType == "Gold")
        {
            _dbContext.MyPortfolio.Remove(holding);
            await _dbContext.SaveChangesAsync();
            TempData["Success"] = "Holding deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetPriceHistory()
    {
        var history = await _dbContext.PriceHistory
            .Where(p => p.MetalType == "Gold")
            .OrderBy(p => p.FetchedAt)
            .Select(p => new { date = p.FetchedAt, price = p.PricePerGram })
            .ToListAsync();

        return Json(history);
    }

    private async Task<MetalViewModel> GetMetalViewModel(string metalType)
    {
        var latestPrice = await _dbContext.PriceHistory
            .Where(p => p.MetalType == metalType)
            .OrderByDescending(p => p.FetchedAt)
            .FirstOrDefaultAsync();

        var portfolioItems = await _dbContext.MyPortfolio
            .Where(p => p.MetalType == metalType)
            .ToListAsync();

        var currentPricePerGram = latestPrice?.PricePerGram ?? 0;

        // Calculate portfolio items with gain/loss
        var portfolioViewItems = portfolioItems.Select(p =>
        {
            var currentWorth = p.QuantityGrams * currentPricePerGram;
            var investedAmount = p.PurchasePrice.HasValue ? p.QuantityGrams * p.PurchasePrice.Value : (decimal?)null;
            var gainLossAmount = investedAmount.HasValue ? currentWorth - investedAmount.Value : (decimal?)null;
            var gainLossPercentage = investedAmount.HasValue && investedAmount.Value > 0
                ? (gainLossAmount!.Value / investedAmount.Value) * 100
                : (decimal?)null;

            return new PortfolioItemViewModel
            {
                Id = p.Id,
                MetalType = p.MetalType,
                QuantityGrams = p.QuantityGrams,
                PurchasePrice = p.PurchasePrice,
                PurchaseDate = p.PurchaseDate,
                Description = p.Description,
                CurrentWorth = currentWorth,
                InvestedAmount = investedAmount,
                GainLossAmount = gainLossAmount,
                GainLossPercentage = gainLossPercentage
            };
        }).ToList();

        var totalQuantity = portfolioItems.Sum(p => p.QuantityGrams);
        var totalWorth = totalQuantity * currentPricePerGram;
        var totalInvested = portfolioViewItems
            .Where(p => p.InvestedAmount.HasValue)
            .Sum(p => p.InvestedAmount!.Value);
        var totalGainLoss = totalWorth - totalInvested;
        var totalGainLossPercentage = totalInvested > 0 ? (totalGainLoss / totalInvested) * 100 : 0;

        var priceHistory = await _dbContext.PriceHistory
            .Where(p => p.MetalType == metalType)
            .OrderByDescending(p => p.FetchedAt)
            .Take(90)
            .ToListAsync();

        return new MetalViewModel
        {
            MetalType = metalType,
            CurrentPricePerGram = currentPricePerGram,
            CurrentPricePerOunce = latestPrice?.PricePerOunce ?? 0,
            LastUpdated = latestPrice?.FetchedAt,
            MyQuantityGrams = totalQuantity,
            MyTotalWorth = totalWorth,
            TotalInvested = totalInvested,
            GainLossAmount = totalGainLoss,
            GainLossPercentage = totalGainLossPercentage,
            PriceHistory = priceHistory,
            PortfolioItems = portfolioViewItems
        };
    }
}
