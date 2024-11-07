using Microsoft.Maui.Controls;
using System.Runtime.CompilerServices;
namespace CurrencyConverterApp
{
    public partial class MainPage : ContentPage
    {
        private string currentInput = string.Empty; //Store the value clicked on the calcuator
        private CurrencyService _currencyService = new CurrencyService();

        public MainPage()
        {
            InitializeComponent();
            InitializeCurrencies(); //Updates the Picker object to display all available Currency Codes
        }

        //This function calls GetCurrenciesAsync function which makes an HTTP call to get Currency Codes
        private async void InitializeCurrencies()
        {
            try
            {
                UpdatingButton.IsVisible = true;
                var currencies = await _currencyService.GetCurrenciesAsync();
                var currencyCodes = currencies.Select(kvp => $"{kvp.Key} - {kvp.Value}").ToList(); //Convert dictionary to list with the text formatted to display both the Key and Value

                //FromCurrencyPicker.ItemsSource = currencyCodes;
                TargetCurrencyPicker.ItemsSource = currencyCodes;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load currencies.", "OK");
            }

            UpdatingButton.IsVisible = false;
        }


        // This function handles the Calculator digit functionality
        private void OnDigitButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            
            currentInput += button.Text;
            GetExchangeRate();
        }

        //This function clears the content of the LCD display on the calculator
        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            currentInput = string.Empty;
            GetExchangeRate();
        }

        //This function updates the LCD display on the calculator if the currentInput is null or empty, it displays 0, otherwise the values pressed on the calculator
        private void UpdateDisplay()
        {
            DisplayLabel.Text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
        }

        // This function gets the ExchangeRate for the two currencies (Base - AUD (Fixed) and Target - User selected)
        private async void GetExchangeRate()
        {
            UpdatingButton.IsVisible = true;
            UpdateDisplay();
            string baseCurrency = "AUD"; //FromCurrencyPicker.SelectedItem.ToString().Split(" - ")[0];

            string targetCurrency = TargetCurrencyPicker.SelectedItem?.ToString().Split(" - ")[0] ?? string.Empty;
            decimal amount = Convert.ToDecimal(DisplayLabel.Text);
            

            //Error chacking
            if (string.IsNullOrEmpty(targetCurrency))
            {
                await DisplayAlert("Error", "Please select exchange currency.", "OK");
                TargetCurrencyPicker.Focus();
                UpdatingButton.IsVisible = false;
                return;
            }

            //Error checking
            if (amount < 0)
            {
                await DisplayAlert("Error", "Please enter amount. (Can't be 0)", "OK");
            }

            //Get the Exchange Rate and Update the Calculator and also handle error if unable to connect to the Web API service
            decimal? exchRate = await _currencyService.GetExchangeRateAsync(baseCurrency, targetCurrency); //This is the call made to get the Exchange Rate for the Base and Target currencies)
            if (exchRate.HasValue)
            {
                //ToCurLabel.Text = exchRate.Value.ToString("0.00000");
                ExchangeRateLabel.Text = $" {targetCurrency}: {exchRate.Value.ToString("0.00###")}\n {targetCurrency} : {(exchRate.Value * amount).ToString("0.00")}";
            }
            else
            {
                await DisplayAlert("Error", "Unable to retrieve exchange rate.", "OK");
            }

            UpdatingButton.IsVisible = false;
        }

        //Removed the Convert Button functionality as I have made the calculator to calculate once it has the amount and currency information provided
        //private void OnConvertButtonClicked(object sender, EventArgs e)
        //{
        //    GetExchangeRate();
        //}

        //This function call will call the GetExchangeRate function when the user selects a target currency
        private void OnTargetCurrencyChanged(object sender, EventArgs e)
        {
            //Call the function to update the Exchange Rate
            GetExchangeRate();
        }

        //This function will call the GetExchangeRate function when the users clicks on the back button on the calculator
        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            
            currentInput = currentInput.Substring(0, Math.Min(currentInput.Length-1, currentInput.Length));
            GetExchangeRate();
        }
    }
}
