using CurrencyRatesApp.WPF.Interfaces;
using CurrencyRatesApp.WPF.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CurrencyRatesApp.WPF.Service
{
    public class RatesService : IRatesService
    {
        private readonly HttpClient _httpClient;
        private const string currentDayRates = "https://api.nbrb.by/exrates/rates?periodicity=0";
        private const string selectedDayRates = "https://api.nbrb.by/exrates/rates?";
        private const string dynamicsRates = "https://api.nbrb.by/ExRates/Rates/Dynamics/";

        public RatesService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Rate[]> GetDailyCurrencyAsync()
        {
            var response = await _httpClient.GetAsync(currentDayRates);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonConvert.DeserializeObject <Rate[]>(content);

            return rates;
        }
        public async Task<Rate[]> GetDailyCurrencyAsync(DateTime? selectedDay)
        {
            var response = await _httpClient.GetAsync($"{selectedDayRates}ondate={selectedDay?.ToString("yyyy-MM-dd")}&periodicity=0");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonConvert.DeserializeObject<Rate[]>(content);

            return rates;
        }


        public async Task<RateShort[]> GetDynamicCurrencyAsync(int curId, DateTime startDate, DateTime endDate)
        {
            var response = await _httpClient.GetAsync($"{dynamicsRates}{curId}?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonConvert.DeserializeObject<RateShort[]>(content);

            return rates;
        }
    }
}
