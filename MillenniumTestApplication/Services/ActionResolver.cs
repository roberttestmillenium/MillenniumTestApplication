using System.Text.Json;
using MillenniumTestApplication.Helpers;
using MillenniumTestApplication.Models;

namespace MillenniumTestApplication.Services
{
    public class ActionResolver
    {
        private List<ActionRule> _rules = new();

        public ActionResolver(string jsonFilePath)
        {
            if (File.Exists(jsonFilePath))
                LoadFromFile(jsonFilePath);
        }

        private void LoadFromFile(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            var rules = JsonSerializer.Deserialize<List<ActionRule>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ActionRule>();

            if (rules == null || !rules.Any())
                throw new InvalidDataException("Nieprawidłowa struktura pliku z regułami.");

            _rules = rules; 
        }

        public void UpdateRules(List<ActionRule> newRules)
        {
            if (newRules == null || !newRules.Any())
                throw new InvalidDataException("Nieprawidłowa struktura danych (brak reguł).");

            _rules = newRules;
        }

        public List<string> Resolve(string cardType, string cardStatus, bool isPinSet)
        {
            var matchedActions = new List<string>();

            foreach (var rule in _rules)
            {
                bool typeMatches = rule.CardTypes.Contains(cardType, StringComparer.OrdinalIgnoreCase);
                bool statusMatches = rule.CardStatuses.Contains(cardStatus, StringComparer.OrdinalIgnoreCase);
                bool pinOk = rule.RequirePin == null || rule.RequirePin == isPinSet;

                if (typeMatches && statusMatches && pinOk)
                {
                    matchedActions.AddRange(rule.Actions);
                }
            }

            var sortedActions = matchedActions
                .Distinct()
                .OrderBy(x => x, new NaturalStringComparer())
                .ToList();

            return sortedActions.Any()
                ? sortedActions
                : new List<string> { "NO ACTION" }; 
        }
    }
}
