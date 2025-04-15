using System.Text.Json.Serialization;

namespace MillenniumTestApplication.Domain.Rules
{
    public class ActionRule
    {
        [JsonPropertyName("actions")]
        public List<string> Actions { get; set; } = new();

        [JsonPropertyName("cardTypes")]
        public List<string> CardTypes { get; set; } = new();

        [JsonPropertyName("cardStatuses")]
        public List<string> CardStatuses { get; set; } = new();

        [JsonPropertyName("requirePin")]
        public bool? RequirePin { get; set; }
    }
}
