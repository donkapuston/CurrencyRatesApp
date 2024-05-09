using CurrencyRatesApp.WPF.Interfaces;
using CurrencyRatesApp.WPF.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRatesApp.WPF.Service
{
    public class RatesService : IRatesService
    {
        private readonly HttpClient _httpClient;
        private const string dayRates = "https://api.nbrb.by/exrates/rates?periodicity=0";
        private const string dateRates = "https://api.nbrb.by/ExRates/Rates/Dynamics/";

        public RatesService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Rate[]> GetDailyCurrency()
        {
            var response = await _httpClient.GetAsync(dayRates);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonConvert.DeserializeObject < Rate[]>(content);

            return rates;
        }

        public async Task<RateShort[]> GetDynamicCurrency(int curId, DateTime startDate, DateTime endDate)
        {
            var response = await _httpClient.GetAsync($"{dateRates}{curId}?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonConvert.DeserializeObject<RateShort[]>(content);

            return rates;
        }
    }
}
