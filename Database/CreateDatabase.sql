-- Create GoldPrice Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GoldPrice')
BEGIN
    CREATE DATABASE GoldPrice;
END
GO

USE GoldPrice;
GO

-- Price History Table (stores historical prices for both gold and silver)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PriceHistory')
BEGIN
    CREATE TABLE PriceHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MetalType NVARCHAR(10) NOT NULL, -- 'Gold' or 'Silver'
        PricePerGram DECIMAL(18,4) NOT NULL,
        PricePerOunce DECIMAL(18,4) NOT NULL,
        Currency NVARCHAR(10) NOT NULL DEFAULT 'NZD',
        FetchedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_PriceHistory_MetalType_FetchedAt ON PriceHistory(MetalType, FetchedAt DESC);
END
GO

-- My Portfolio Table (stores user's metal holdings)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MyPortfolio')
BEGIN
    CREATE TABLE MyPortfolio (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MetalType NVARCHAR(10) NOT NULL, -- 'Gold' or 'Silver'
        QuantityGrams DECIMAL(18,4) NOT NULL,
        PurchasePrice DECIMAL(18,4) NULL, -- Optional: price paid per gram
        PurchaseDate DATE NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_MyPortfolio_MetalType ON MyPortfolio(MetalType);
END
GO

-- Insert default portfolio entries if empty
IF NOT EXISTS (SELECT 1 FROM MyPortfolio WHERE MetalType = 'Gold')
BEGIN
    INSERT INTO MyPortfolio (MetalType, QuantityGrams, Description)
    VALUES ('Gold', 0, 'My Gold Holdings');
END

IF NOT EXISTS (SELECT 1 FROM MyPortfolio WHERE MetalType = 'Silver')
BEGIN
    INSERT INTO MyPortfolio (MetalType, QuantityGrams, Description)
    VALUES ('Silver', 0, 'My Silver Holdings');
END
GO
