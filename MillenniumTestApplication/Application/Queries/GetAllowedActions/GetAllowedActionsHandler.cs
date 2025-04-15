using MediatR;
using MillenniumTestApplication.Application.Queries.GetAllowedActions;
using MillenniumTestApplication.Domain.Interfaces;
using MillenniumTestApplication.Shared.Helpers;

namespace MillenniumTestApplication.Application.Queries.GetAllowedActionsHandler
{
    public class GetAllowedActionsHandler : IRequestHandler<GetAllowedActionsQuery, GetAllowedActionsResult>
    {
        private readonly ICardReader _cardReader;
        private readonly IActionResolver _resolver;

        public GetAllowedActionsHandler(ICardReader cardReader, IActionResolver resolver)
        {
            _cardReader = cardReader;
            _resolver = resolver;
        }

        public async Task<GetAllowedActionsResult> Handle(GetAllowedActionsQuery request, CancellationToken cancellationToken)
        {
            var userId = StringNormalizationHelper.NormalizeInput(request.UserId);
            var cardNumber = StringNormalizationHelper.NormalizeInput(request.CardNumber);

            var card = await _cardReader.GetCardDetails(userId, cardNumber)
                       ?? throw new InvalidOperationException("Card not found");

            var actions = _resolver.Resolve(
                card.CardType.ToString(),
                card.CardStatus.ToString(),
                card.IsPinSet
            );

            return new GetAllowedActionsResult(card, actions);
        }
    }
}
