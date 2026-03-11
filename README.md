# Precious Metals Tracker

A web application to track Gold and Silver prices in NZD (New Zealand Dollars) and manage your precious metals portfolio.

## Features

- **Real-time Price Fetching** - Get current gold and silver prices from GoldAPI (converted to NZD)
- **Price History Chart** - Visual chart showing price trends over time
- **Portfolio Management** - Add, edit, and delete your metal holdings
- **Gain/Loss Tracking** - Track your investment performance with percentage gain/loss
- **Present Value Calculation** - See your portfolio's current worth based on live prices

## Tech Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** SQL Server
- **ORM:** Entity Framework Core 8.0
- **Frontend:** Bootstrap 5, Bootstrap Icons, Chart.js
- **API:** GoldAPI.io for precious metal prices

## Screenshots

### Gold Page
- Current gold price per gram and per troy ounce
- Portfolio summary with total worth
- Gain/Loss card showing investment performance
- Holdings table with edit/delete functionality
- Price history chart

### Silver Page
- Same features as Gold page for silver tracking

## Database Schema

### PriceHistory
| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary key |
| MetalType | NVARCHAR(10) | 'Gold' or 'Silver' |
| PricePerGram | DECIMAL(18,4) | Price per gram in NZD |
| PricePerOunce | DECIMAL(18,4) | Price per troy ounce in NZD |
| Currency | NVARCHAR(10) | Currency code (NZD) |
| FetchedAt | DATETIME2 | When price was fetched |
| CreatedAt | DATETIME2 | Record creation time |

### MyPortfolio
| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary key |
| MetalType | NVARCHAR(10) | 'Gold' or 'Silver' |
| QuantityGrams | DECIMAL(18,4) | Amount in grams |
| PurchasePrice | DECIMAL(18,4) | Price paid per gram (for gain/loss) |
| PurchaseDate | DATE | When purchased |
| Description | NVARCHAR(500) | Optional description |
| CreatedAt | DATETIME2 | Record creation time |
| UpdatedAt | DATETIME2 | Last update time |

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "GoldPriceDb": "Server=YOUR_SERVER;Database=GoldPrice;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

### GoldAPI Key
The API key is configured in `Services/GoldApiService.cs`. Get your free API key from [goldapi.io](https://www.goldapi.io/).

## Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd GoldPrice
   ```

2. **Update connection string** in `appsettings.json`

3. **Run database script**
   Execute `Database/CreateDatabase.sql` on your SQL Server

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open browser** to `http://localhost:5043`

## Usage

1. **Fetch Prices** - Click "Fetch Latest Price" to get current prices
2. **Add Holdings** - Click "Add Gold/Silver Holding" and enter:
   - Quantity in grams
   - Purchase price per gram (for gain/loss tracking)
   - Purchase date (optional)
   - Description (optional)
3. **Edit Holdings** - Click the pencil icon to modify existing holdings
4. **Delete Holdings** - Click the trash icon to remove holdings
5. **View Charts** - Price history builds as you fetch prices over time

## Project Structure

```
GoldPrice/
├── Controllers/
│   ├── GoldController.cs
│   └── SilverController.cs
├── Data/
│   └── GoldPriceDbContext.cs
├── Database/
│   └── CreateDatabase.sql
├── Models/
│   ├── MetalViewModel.cs
│   ├── MyPortfolio.cs
│   └── PriceHistory.cs
├── Services/
│   └── GoldApiService.cs
├── Views/
│   ├── Gold/
│   │   └── Index.cshtml
│   ├── Silver/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── appsettings.json
└── Program.cs
```

## License

MIT License
