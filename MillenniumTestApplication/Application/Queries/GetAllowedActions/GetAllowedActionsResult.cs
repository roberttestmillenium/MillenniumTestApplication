using MillenniumTestApplication.Domain.Entities;

namespace MillenniumTestApplication.Application.Queries.GetAllowedActions
{
    public record GetAllowedActionsResult(CardDetails Card, List<string> Actions);
}
