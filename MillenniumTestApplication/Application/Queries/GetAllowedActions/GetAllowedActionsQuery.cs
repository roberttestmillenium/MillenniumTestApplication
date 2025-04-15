using MediatR;

namespace MillenniumTestApplication.Application.Queries.GetAllowedActions
{
    public record GetAllowedActionsQuery(string UserId, string CardNumber)
    : IRequest<GetAllowedActionsResult>;
}
