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
        private static string API_KEY = "key";
        
        public List<Currency> coins = new List<Currency>();
        public CoinMarketConnector()
        {
            coins = GetCoins(MakeAPICall());
        }
        /// <summary>
        /// Получение котировок криптовалют
        /// </summary>
        /// <returns>Список котировок</returns>
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
        /// Получение логотипов криптовалют
        /// </summary>
        /// <param name="list">Список криптовалют, которые получаем из метода MakeAPICall</param>
        /// <returns>Логотипы валют</returns>
        public static List<Currency> GetInfoAPI(List<Currency> list)
        {
            List<Currency> result = new List<Currency>();
            string ids = "";
            
            foreach (var item in list)
            {
                ids = ids + item.Id + ",";
            }

            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/info");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["id"] = ids.Remove(ids.Length - 1);
            queryString["aux"] = "logo";

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            string JSONResponse = client.DownloadString(URL.ToString());

            JObject parsedResponse = JObject.Parse(JSONResponse);
            JObject data = JObject.FromObject(parsedResponse["data"]);
            List<JToken> items = data.Values().ToList();

            Dictionary<string, string> imagesMap = new Dictionary<string, string>();

            foreach (var i in items)
            {
                imagesMap.Add(i["id"].ToObject<string>(), i["logo"].ToObject<string>());
            }

            foreach (Currency currency in list)
            {
                currency.Logo = imagesMap[currency.Id];
                result.Add(currency);
            }
            return result;
        }
        /// <summary>
        /// Получение списка криптовалют.
        /// </summary>
        /// <param name="JSONString">Список криптовалют</param>
        /// <returns>Список криптовалют</returns>
        /// 
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

            return new Currency(item["id"].ToObject<string>(), item["name"].ToObject<string>(), item["symbol"].ToObject<string>(), "some link", price, 
                                changeHour, changeDay, capitalizationUSD, lastUpdate);
        }
    }
}
