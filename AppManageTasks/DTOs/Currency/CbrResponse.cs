using System.Text.Json.Serialization;

namespace AppManageTasks.DTOs.Currency
{
    public class CbrResponse
    {
        [JsonPropertyName("Date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("PreviousDate")]
        public DateTime PreviousDate { get; set; }

        [JsonPropertyName("PreviousURL")]
        public string PreviousURL { get; set; }

        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("Valute")]
        public Dictionary<string, CurrencyRate> Valute { get; set; }
    }
}
