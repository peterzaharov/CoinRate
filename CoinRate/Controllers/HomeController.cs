using CoinRate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace CoinRate.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var connector = new CoinMarketConnector();
            List<Currency> coins = connector.coins;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Текущая цена (USD)" ? "price_desc" : "Текущая цена (USD)";
            ViewBag.Persent1hSortParm = sortOrder == "Изменение за 1 час (%)" ? "persent1h_desc" : "Изменение за 1 час (%)";
            ViewBag.Persent24hSortParm = sortOrder == "Изменение за 24 часа (%)" ? "persent24h_desc" : "Изменение за 24 часа (%)";
            ViewBag.MarketCapSortParm = sortOrder == "Капитализация" ? "market_cap_desc" : "Капитализация";
            ViewBag.DateSortParm = sortOrder == "Время обновления данных" ? "date_desc" : "Время обновления данных";
            
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var sortedListCoins = from s in coins
                                  select s;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                sortedListCoins = sortedListCoins.Where(s => s.Name.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    sortedListCoins = sortedListCoins.OrderByDescending(s => s.Name);
                    break;
                case "Текущая цена (USD)":
                    sortedListCoins = sortedListCoins.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    sortedListCoins = sortedListCoins.OrderByDescending(s => s.Price);
                    break;
                case "Изменение за 1 час (%)":
                    sortedListCoins = sortedListCoins.OrderBy(s => s.PercentChange1h);
                    break;
                case "persent1h_desc":
                    sortedListCoins = sortedListCoins.OrderByDescending(s => s.PercentChange1h);
                    break;
                case "Изменение за 24 часа (%)":
                    sortedListCoins = sortedListCoins.OrderBy(s => s.PercentChange24h);
                    break;
                case "persent24h_desc":
                    sortedListCoins = sortedListCoins.OrderByDescending(s => s.PercentChange24h);
                    break;
                case "Капитализация":
                    sortedListCoins = sortedListCoins.OrderBy(s => s.MarketCup);
                    break;
                case "market_cap_desc":
                    sortedListCoins = sortedListCoins.OrderByDescending(s => s.MarketCup);
                    break;
                case "Время обновления данных":
                    sortedListCoins = sortedListCoins.OrderBy(s => s.LastUpdated);
                    break;
                case "date_desc":
                    sortedListCoins = sortedListCoins.OrderByDescending(s => s.LastUpdated);
                    break;
                default:
                    sortedListCoins = sortedListCoins.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);

            List<Currency> currencies = CoinMarketConnector.GetInfoAPI(sortedListCoins.ToList());

            return View(currencies.ToPagedList(pageNumber, pageSize));
        }
    }
}