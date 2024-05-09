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
        Task<Rate[]> GetDailyCurrency();
        Task<RateShort[]> GetDynamicCurrency(int curId, DateTime startDate, DateTime endDate);
    }
}
