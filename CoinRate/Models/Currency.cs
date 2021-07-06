using System;

namespace CoinRate.Models
{
    public class Currency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Logo { get; set; }
        public decimal Price { get; set; }
        public double PercentChange1h { get; set; }
        public double PercentChange24h { get; set; }
        public decimal MarketCup { get; set; }
        public DateTime LastUpdated { get; set; }
        public Currency() { }
        public Currency(string id, string name, string symbol, string logo, decimal currentPriceUSD, 
                        double percentChange1h, double percentChange24h, decimal marketCap, DateTime lastUpdated)
        {
            Id = id;
            Name = name;
            Symbol = symbol;
            Logo = logo;
            Price = currentPriceUSD;
            PercentChange1h = percentChange1h;
            PercentChange24h = percentChange24h;
            MarketCup = marketCap;
            LastUpdated = lastUpdated;
        }
    }
}