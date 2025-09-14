using AppManageTasks.DTOs.Currency;
using AppManageTasks.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppManageTasks.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// Получить курс конкретной валюты
        /// </summary>
        [HttpGet("{currencyCode}")]
        public async Task<ActionResult<CurrencyRate>> GetCurrencyRate(string currencyCode = "USD")
        {
            try
            {
                var rate = await _currencyService.GetCurrencyRateAsync(currencyCode);
                return Ok(rate);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить все курсы валют
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CbrResponse>> GetAllCurrencyRates()
        {
            try
            {
                var rates = await _currencyService.GetAllCurrencyRatesAsync();
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}