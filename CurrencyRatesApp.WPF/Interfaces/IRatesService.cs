using CurrencyRatesApp.WPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRatesApp.WPF.Interfaces
{
    public interface IRatesService
    {
        Task<Rate[]> GetDailyCurrencyAsync();
        Task<Rate[]> GetDailyCurrencyAsync(DateTime? startDate);
        Task<RateShort[]> GetDynamicCurrencyAsync(int curId, DateTime startDate, DateTime endDate);
    }
}
