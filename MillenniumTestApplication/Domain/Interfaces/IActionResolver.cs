using MillenniumTestApplication.Domain.Rules;

namespace MillenniumTestApplication.Domain.Interfaces
{
    public interface IActionResolver
    {
        List<string> Resolve(string cardType, string cardStatus, bool isPinSet);

        void UpdateRules(List<ActionRule> newRules);
    }
}
