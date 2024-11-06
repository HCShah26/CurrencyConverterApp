using Microsoft.Maui.Controls;
namespace CurrencyConverterApp
{
    public partial class MainPage : ContentPage
    {
        private string currentInput = string.Empty;
        private CurrencyService _currencyService = new CurrencyService();

        public MainPage()
        {
            InitializeComponent();
            InitializeCurrencies();
        }

        private async void InitializeCurrencies()
        {
            try
            {
                var currencies = await _currencyService.GetCurrenciesAsync();
                var currencyCodes = currencies.Select(kvp => $"{kvp.Key} - {kvp.Value}").ToList();

                //FromCurrencyPicker.ItemsSource = currencyCodes;
                ToCurrencyPicker.ItemsSource = currencyCodes;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load currencies.", "OK");
            }
        }



        private void OnDigitButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (currentInput.Length < 8)
            {
                currentInput += button.Text;
                UpdateDisplay();
                GetExchangeRate();
            }
        }

        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            currentInput = string.Empty;
            UpdateDisplay();
            GetExchangeRate();
        }

        private void UpdateDisplay()
        {
            DisplayLabel.Text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
        }


        private async void GetExchangeRate()
        {
            string fromCurrency = "AUD";//FromCurrencyPicker.SelectedItem.ToString().Split(" - ")[0];

            string toCurrency = ToCurrencyPicker.SelectedItem?.ToString().Split(" - ")[0] ?? string.Empty;
            decimal amount = Convert.ToDecimal(DisplayLabel.Text);
            if (string.IsNullOrEmpty(toCurrency))
            {
                await DisplayAlert("Error", "Please select exchange currency.", "OK");
                ToCurrencyPicker.Focus();
                return;
            }
            if (amount < 0)
            {
                await DisplayAlert("Error", "Please enter amount. (Can't be 0)", "OK");
            }

            decimal? exchRate = await _currencyService.GetExchangeRateAsync(fromCurrency, toCurrency);
            if (exchRate.HasValue)
            {
                //ToCurLabel.Text = exchRate.Value.ToString("0.00000");
                ExchangeRateLabel.Text = $" {toCurrency} Rate: {exchRate.Value.ToString("0.00###")}\n {toCurrency} : {(exchRate.Value * amount).ToString("0.00")}";
            }
            else
            {
                await DisplayAlert("Error", "Unable to retrieve exchange rate.", "OK");
            }

        }
        private void OnConvertButtonClicked(object sender, EventArgs e)
        {
            GetExchangeRate();
        }

        private void OnFromCurrencyChanged(object sender, EventArgs e)
        {
            // Handle currency change logic if needed
            GetExchangeRate();
        }

        private void OnToCurrencyChanged(object sender, EventArgs e)
        {
            // Handle currency change logic if needed
            GetExchangeRate();
        }
    }
}
