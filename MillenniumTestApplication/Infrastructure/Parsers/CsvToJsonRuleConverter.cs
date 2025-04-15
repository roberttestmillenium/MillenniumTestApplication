using MillenniumTestApplication.Domain.Enums;
using MillenniumTestApplication.Domain.Rules;
using MillenniumTestApplication.Shared.Comparers;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MillenniumTestApplication.Infrastructure.Parsers
{
    public static class CsvToJsonRuleConverter
    {
        public static List<ActionRule> ConvertToMemory(string csvPath)
        {
            var rawRecords = ParseCsvFile(csvPath);
            var ruleMap = BuildRuleMap(rawRecords);
            return MapToActionRules(ruleMap);
        }

        private static IEnumerable<IDictionary<string, object>> ParseCsvFile(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                PrepareHeaderForMatch = args => args.Header.ToUpperInvariant()
            };

            List<dynamic> records;

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<dynamic>().ToList();
            }

            return records.Select(row => (IDictionary<string, object>)row);
        }

        private static Dictionary<(string cardType, string cardStatus, bool requirePin), HashSet<string>> BuildRuleMap(IEnumerable<IDictionary<string, object>> records)
        {
            var rules = new Dictionary<(string cardType, string cardStatus, bool requirePin), HashSet<string>>();
            var cardTypes = Enum.GetNames(typeof(CardType));
            var statuses = Enum.GetNames(typeof(CardStatus));

            foreach (var record in records)
            {
                var action = GetUpper(record, "ALLOWED ACTION");
                if (string.IsNullOrWhiteSpace(action)) continue;

                var validCardTypes = GetMatchingCardTypes(record, cardTypes);
                var validStatuses = GetMatchingStatusesWithPin(record, statuses);

                foreach (var cardType in validCardTypes)
                {
                    foreach (var (status, requiresPin) in validStatuses)
                    {
                        AddRule(rules, cardType, status, requiresPin, action);
                    }
                }
            }

            return rules;
        }

        private static IEnumerable<string> GetMatchingCardTypes(IDictionary<string, object> row, IEnumerable<string> cardTypes)
        {
            foreach (var type in cardTypes)
            {
                if (GetUpper(row, type.ToUpperInvariant()) == "TAK")
                    yield return type;
            }
        }

        private static IEnumerable<(string status, bool requirePin)> GetMatchingStatusesWithPin(IDictionary<string, object> row, IEnumerable<string> statuses)
        {
            foreach (var status in statuses)
            {
                var value = GetUpper(row, status.ToUpperInvariant());
                if (string.IsNullOrWhiteSpace(value) || value == "NIE") continue;

                if (value.Contains("GDY PIN", StringComparison.OrdinalIgnoreCase))
                {
                    yield return (status, true);
                }
                else
                {
                    yield return (status, true);
                    yield return (status, false);
                }
            }
        }

        private static void AddRule(Dictionary<(string cardType, string cardStatus, bool requirePin), HashSet<string>> rules,
                                    string cardType, string status, bool requirePin, string action)
        {
            var key = (cardType, status, requirePin);
            if (!rules.TryGetValue(key, out var set))
            {
                set = new HashSet<string>();
                rules[key] = set;
            }
            set.Add(action);
        }

        private static List<ActionRule> MapToActionRules(Dictionary<(string cardType, string cardStatus, bool requirePin), HashSet<string>> ruleMap)
        {
            return ruleMap
                .Select(kvp => new ActionRule
                {
                    CardTypes = new List<string> { kvp.Key.cardType },
                    CardStatuses = new List<string> { kvp.Key.cardStatus },
                    Actions = kvp.Value.OrderBy(a => a, new NaturalStringComparer()).ToList(),
                    RequirePin = kvp.Key.requirePin
                })
                .ToList();
        }

        private static string? GetUpper(IDictionary<string, object> row, string key)
            => row.TryGetValue(key, out var val) ? val?.ToString()?.Trim().ToUpperInvariant() : null;
    }
}
