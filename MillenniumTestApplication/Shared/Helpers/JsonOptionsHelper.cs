using System.Text.Json;

namespace MillenniumTestApplication.Shared.Helpers
{
    public static class JsonOptionsHelper
    {
        public static readonly JsonSerializerOptions CaseInsensitive = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
