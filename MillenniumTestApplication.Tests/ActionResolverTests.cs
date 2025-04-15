using MillenniumTestApplication.Domain.Rules;
using MillenniumTestApplication.Domain.Services;

namespace MillenniumTestApplication.Tests
{
    public class ActionResolverTests
    {
        [Fact]
        public void Resolve_ShouldReturnAllMatchingActions()
        {
            // Arrange
            var rulesJson = """
                        [
                          {
                            "cardTypes": [ "PREPAID" ],
                            "cardStatuses": [ "ACTIVE" ],
                            "requirePin": true,
                            "actions": [ "ACTION1" ]
                          },
                          {
                            "cardTypes": [ "PREPAID" ],
                            "cardStatuses": [ "ACTIVE" ],
                            "actions": [ "ACTION2" ]
                          }
                        ]
                        """;

            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, rulesJson);

            var resolver = new ActionResolver(tempPath);

            var result = resolver.Resolve("PREPAID", "ACTIVE", true);

            Assert.Contains("ACTION1", result);
            Assert.Contains("ACTION2", result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Resolve_ShouldReturnEmpty_WhenNoMatch()
        {
            string rulesJson = """
                                [
                                  {
                                    "cardTypes": [ "CREDIT" ],
                                    "cardStatuses": [ "BLOCKED" ],
                                    "requirePin": true,
                                    "actions": [ "ACTION99" ]
                                  }
                                ]
                                """;

            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, rulesJson);

            var resolver = new ActionResolver(tempPath);

            var result = resolver.Resolve("DEBIT", "ACTIVE", false);

            Assert.Empty(result);
        }
    }
}
