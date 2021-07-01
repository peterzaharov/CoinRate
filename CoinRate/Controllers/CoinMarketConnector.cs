using CoinRate.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CoinRate.Controllers
{
    public class CoinMarketConnector
    {
        private string API_KEY = "key";
        
        public List<Currency> coins = new List<Currency>();
        public CoinMarketConnector()
        {
            coins = GetCoins(MakeAPICall());
        }
        private string MakeAPICall()
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = "1";
            queryString["limit"] = "10";
            queryString["convert"] = "USD";

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            return client.DownloadString(URL.ToString());
        }
        /// <summary>
        /// Получение списка криптовалют.
        /// </summary>
        /// <param name="JSONString">Список криптовалют</param>
        /// <returns>Список криптовалют</returns>
        private List<Currency> GetCoins(string JSONString)
        {
            List<Currency> result = new List<Currency>();
            JObject parsedResponse = JObject.Parse(JSONString);
            JArray data = JArray.FromObject(parsedResponse["data"]);
            foreach (JObject item in data)
            {
                result.Add(GetCoin(item));
            }
            return result;
        }
        /// <summary>
        /// Получение одного экземпляра криптовалюты.
        /// </summary>
        /// <param name="item">Экземляр криптовалюты</param>
        /// <returns>Экземпляр криптовалюты</returns>
        private Currency GetCoin(JObject item)
        {
            decimal price = Math.Round(item["quote"]["USD"]["price"].ToObject<decimal>(), 2);
            double changeHour = Math.Round(item["quote"]["USD"]["percent_change_1h"].ToObject<double>(), 2);
            double changeDay = Math.Round(item["quote"]["USD"]["percent_change_24h"].ToObject<double>(), 2);
            decimal capitalizationUSD = Math.Round(item["quote"]["USD"]["market_cap"].ToObject<decimal>(), 3);
            DateTime lastUpdate = item["last_updated"].ToObject<DateTime>();

            return new Currency(item["name"].ToObject<string>(), item["symbol"].ToObject<string>(), "some link", price, 
                                changeHour, changeDay, capitalizationUSD, lastUpdate);
        }
    }
}