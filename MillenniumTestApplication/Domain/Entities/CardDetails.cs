using MillenniumTestApplication.Domain.Enums;

namespace MillenniumTestApplication.Domain.Entities
{
    public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);
}
