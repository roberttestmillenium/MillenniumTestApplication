using System.Text.Json;
using MillenniumTestApplication.Domain.Interfaces;
using MillenniumTestApplication.Domain.Rules;
using MillenniumTestApplication.Shared.Comparers;
using MillenniumTestApplication.Shared.Helpers;

namespace MillenniumTestApplication.Domain.Services
{
    public class ActionResolver : IActionResolver
    {
        private List<ActionRule> _rules = new();

        public ActionResolver(string jsonFilePath)
        {
            LoadFromFile(jsonFilePath);
        }

        public void LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            var rules = JsonSerializer.Deserialize<List<ActionRule>>(json, JsonOptionsHelper.CaseInsensitive);

            if (rules == null || rules.Count == 0)
                throw new InvalidDataException("Nieprawidłowa struktura pliku z regułami.");

            _rules = rules;
        }

        public void UpdateRules(List<ActionRule> newRules)
        {
            if (newRules == null || newRules.Count == 0)
                throw new InvalidDataException("Nieprawidłowe dane – lista reguł jest pusta.");

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

            return matchedActions
                .Distinct()
                .OrderBy(x => x, new NaturalStringComparer())
                .ToList();
        }
    }
}