using CsvHelper;
using CsvHelper.Configuration;
using MillenniumTestApplication.Models;
using System.Globalization;

namespace MillenniumTestApplication.Helpers
{
    public static class CsvToJsonRuleConverter
    {
        public static List<ActionRule> ConvertToMemory(string csvPath)
        {
            var cardTypes = new[] { "PREPAID", "DEBIT", "CREDIT" };
            var statuses = new[] { "ORDERED", "INACTIVE", "ACTIVE", "RESTRICTED", "BLOCKED", "EXPIRED", "CLOSED" };

            var rules = new Dictionary<(string cardType, string cardStatus, bool requirePin), HashSet<string>>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                PrepareHeaderForMatch = args => args.Header.ToUpperInvariant()
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<dynamic>();

            foreach (var row in records)
            {
                var dict = (IDictionary<string, object>)row;

                if (!dict.TryGetValue("ALLOWED ACTION", out var actionObj) || actionObj == null)
                    continue;

                string actionName = actionObj.ToString()?.Trim().ToUpper();
                if (string.IsNullOrWhiteSpace(actionName))
                    continue;

                foreach (var ct in cardTypes)
                {
                    var typeVal = dict.ContainsKey(ct) ? dict[ct]?.ToString()?.ToUpper() : null;
                    if (typeVal != "TAK") continue;

                    foreach (var st in statuses)
                    {
                        var statusVal = dict.ContainsKey(st) ? dict[st]?.ToString()?.ToUpper() : null;
                        if (string.IsNullOrWhiteSpace(statusVal) || statusVal == "NIE") continue;

                        bool requirePin = statusVal.Contains("GDY PIN");

                        var key = (ct, st, requirePin);
                        if (!rules.ContainsKey(key))
                            rules[key] = new HashSet<string>();

                        rules[key].Add(actionName);
                    }
                }
            }

            return rules
                .Select(kvp => new ActionRule
                {
                    CardTypes = new List<string> { kvp.Key.cardType },
                    CardStatuses = new List<string> { kvp.Key.cardStatus },
                    Actions = kvp.Value.OrderBy(a => a, new NaturalStringComparer()).ToList(),
                    RequirePin = kvp.Key.requirePin
                })
                .ToList();
        }
    }
}
