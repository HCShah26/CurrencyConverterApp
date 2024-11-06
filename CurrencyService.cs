using Newtonsoft.Json;
using System.Net.Http;

namespace CurrencyConverterApp
{
    public class CurrencyService
    {
        private const string CurrencyApiUrl = "https://openexchangerates.org/api/currencies.json";
        private const string ExchangeRateApiUrl = "https://openexchangerates.org/api/latest.json?app_id=3ac1cc137ee24c81bdea813fdb58b1ed";

        // Method to fetch currency symbols
        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(CurrencyApiUrl);
                string responseStr = await response.Content.ReadAsStringAsync();

                var currencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseStr);
                return currencies;
            }
        }

        // Method to fetch exchange rates
        public async Task<decimal?> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync($"{ExchangeRateApiUrl}&symbols={fromCurrency},{toCurrency}");
                var data = JsonConvert.DeserializeObject<ExchangeRates>(response);

                if (data.Rates.TryGetValue(fromCurrency, out decimal baseRate) && data.Rates.TryGetValue(toCurrency, out decimal rate))
                {
                    return rate / baseRate;
                }
                return null;
            }
        }
    }

}
