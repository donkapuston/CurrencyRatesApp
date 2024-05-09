using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRatesApp.WPF.Interfaces
{
    public interface IMessageService
    {
        void ShowMessage(string message);
        bool ShowConfirmation(string message, string title);
    }
}
