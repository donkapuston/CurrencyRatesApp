using CurrencyRatesApp.WPF.Interfaces;
using CurrencyRatesApp.WPF.Model;
using CurrencyRatesApp.WPF.Service;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CurrencyRatesApp.WPF.ViewModel
{
    public class RatesViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ICommand GetDailyCurrensyCommand {  get; private set; }
        public ICommand GetDynamicCurrencyCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand SaveNewCurrFileCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand SaveCurrentRateCommand { get; private set; }
        private readonly IMessageService _messageService;
        private readonly IRatesService _ratesService;
        public ObservableCollection<Rate> Rates { get; set; }
        public ObservableCollection<RateShort> Shorts { get; set; }
        private string FileName;

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                UpdateDateValidation();
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                UpdateDateValidation();
            }
        }

        private bool _isStartDateAfterEndDate;
        public bool IsStartDateAfterEndDate
        {
            get { return _isStartDateAfterEndDate; }
            set
            {
                _isStartDateAfterEndDate = value;
                OnPropertyChanged(nameof(IsStartDateAfterEndDate));               
            }
        }

        private DateTime? _currDate;
        public DateTime? CurrencyDate
        {
            get { return _currDate; }
            set
            {
                _currDate = value;
                OnPropertyChanged(nameof(CurrencyDate));
            }
        }

        private Rate _selectedRate;
        public Rate SelectedRate
        {
            get { return _selectedRate; }
            set
            {
                _selectedRate = value;
                OnPropertyChanged(nameof(SelectedRate));
                GetDynamicCurrencyCommand.Execute(null);
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private bool _useSelectedDate;
        public bool UseSelectedDate
        {
            get { return _useSelectedDate; }
            set
            {
                _useSelectedDate = value;
                OnPropertyChanged(nameof(UseSelectedDate));
            }
        }
        private bool _useToday;
        public bool UseToday
        {
            get { return !_useSelectedDate; }
            set
            {
                _useSelectedDate = !value;
                OnPropertyChanged(nameof(UseToday));
                OnPropertyChanged(nameof(UseSelectedDate));
            }
        }

        public RatesViewModel(IMessageService messageService, IRatesService ratesService)
        {
            _messageService = messageService;
            _ratesService = ratesService;
            Rates = new ObservableCollection<Rate>();
            Shorts = new ObservableCollection<RateShort>();
            GetDailyCurrensyCommand = new RelayCommand(param => GetDailyCurrency());
            CloseWindowCommand = new RelayCommand(param => CloseWindow());
            GetDynamicCurrencyCommand = new RelayCommand(param => GetDynamicCurrency());
            OpenFileCommand = new RelayCommand(param => LoadData(Rates));
            SaveNewCurrFileCommand = new RelayCommand(param => SaveDataAsJson(Rates, Description));
            SaveCurrentRateCommand = new RelayCommand(param => SaveDataAsJson(Rates, FileName));
        }

        public async void GetDailyCurrency()
        {
            Rates.Clear();
            try
            {
                IEnumerable<Rate> response;
                if (UseSelectedDate)
                {
                    if (CurrencyDate.HasValue)
                    {
                        response = await _ratesService.GetDailyCurrencyAsync(CurrencyDate);
                    }
                    else
                    {
                        response = null;
                        _messageService.ShowMessage("Не выбрана дата");
                    }                  
                }
                else
                {
                    response = await _ratesService.GetDailyCurrencyAsync();
                }
                
                foreach (var rate in response)
                {
                    Rates.Add(rate);
                }
                FileName = DateTime.Now.ToString("fffffff");
                SaveDataAsJson(Rates, FileName);
            }
            catch (Exception ex)
            {              
                _messageService.ShowMessage($"An error occurred: {ex.Message}");
            }
        }
        public void CloseWindow()
        {
            Application.Current.Shutdown();
        }
        public async void GetDynamicCurrency()
        {
            Shorts.Clear();
            if (SelectedRate != null && StartDate != null && EndDate != null)
            {
                var curId = SelectedRate.Cur_ID;
                var startDate = StartDate.Value;
                var endDate = EndDate.Value;

                var result = await _ratesService.GetDynamicCurrencyAsync(curId, startDate, endDate);

                foreach (var rate in result)
                {
                    Shorts.Add(rate);
                }              
            }
        }
        public void SaveDataAsJson<T>(ObservableCollection<T> data, string fileName)
        {
            if(!string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(FileName))
            {
                var json = JsonConvert.SerializeObject(data);
                string filePath = $"{Directory.GetCurrentDirectory()}\\Currenncy";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                File.WriteAllText($"{filePath}\\{fileName}.json", json);
            }
            else
            {
                _messageService.ShowMessage("Введите описание"); 
            }
        }

        public void LoadData<T>(ObservableCollection<T> collection)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                FileName = Path.GetFileNameWithoutExtension(filePath);
                var json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<List<T>>(json);

                collection.Clear();
                foreach (var item in data)
                {
                    collection.Add(item);
                }
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName =="Description" && string.IsNullOrEmpty(Description))
                {
                    return "Описание не может быть пустым";
                }
                return null;
            }
        }
        private void UpdateDateValidation()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                IsStartDateAfterEndDate = StartDate > EndDate;
            }
            else
            {
                IsStartDateAfterEndDate = false; 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
