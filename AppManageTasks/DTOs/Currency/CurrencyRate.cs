using System.Text.Json.Serialization;

namespace AppManageTasks.DTOs.Currency
{
    public class CurrencyRate
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; }

        [JsonPropertyName("NumCode")]
        public string NumCode { get; set; }

        [JsonPropertyName("CharCode")]
        public string CharCode { get; set; }

        [JsonPropertyName("Nominal")]
        public int Nominal { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Value")]
        public decimal Value { get; set; }

        [JsonPropertyName("Previous")]
        public decimal Previous { get; set; }
    }
}
