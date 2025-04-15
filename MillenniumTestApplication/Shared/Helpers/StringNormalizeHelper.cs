using System.Globalization;

namespace MillenniumTestApplication.Shared.Helpers
{
    public static class StringNormalizationHelper
    {
        public static string NormalizeInput(string input)
        {
            if(string.IsNullOrWhiteSpace(input)) 
                return input;

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
        }
    }
}
