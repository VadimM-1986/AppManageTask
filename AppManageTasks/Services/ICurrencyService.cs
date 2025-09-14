using AppManageTasks.DTOs.Currency;

namespace AppManageTasks.Services
{
    public interface ICurrencyService
    {
        Task<CurrencyRate> GetCurrencyRateAsync(string currencyCode = "USD");
        Task<CbrResponse> GetAllCurrencyRatesAsync();
    }
}
