using AppManageTasks.DTOs.Currency;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AppManageTasks.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public CurrencyService(
            HttpClient httpClient,
            ILogger<CurrencyService> logger,
            IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<CbrResponse> GetAllCurrencyRatesAsync()
        {
            var cacheKey = "currency_rates_all";

            if (_memoryCache.TryGetValue(cacheKey, out CbrResponse cachedRates))
            {
                return cachedRates;
            }

            try
            {
                var response = await _httpClient.GetAsync("https://www.cbr-xml-daily.ru/daily_json.js");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var rates = JsonSerializer.Deserialize<CbrResponse>(content);

                _memoryCache.Set(cacheKey, rates, _cacheDuration);

                return rates;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CurrencyRate> GetCurrencyRateAsync(string currencyCode = "USD")
        {
            var cacheKey = $"currency_rate_{currencyCode}";

            if (_memoryCache.TryGetValue(cacheKey, out CurrencyRate cachedRate))
            {
                return cachedRate;
            }

            var allRates = await GetAllCurrencyRatesAsync();

            if (allRates.Valute.TryGetValue(currencyCode, out var rate))
            {
                _memoryCache.Set(cacheKey, rate, _cacheDuration);
                return rate;
            }

            throw new ArgumentException($"Currency code {currencyCode} not found");
        }
    }
}