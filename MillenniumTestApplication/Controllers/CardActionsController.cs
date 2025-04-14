using Microsoft.AspNetCore.Mvc;
using MillenniumTestApplication.Helpers;
using MillenniumTestApplication.Models;
using MillenniumTestApplication.Services;

namespace MillenniumTestApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardActionsController : ControllerBase
    {
        private readonly CardService _cardService;
        private readonly ActionResolver _actionResolver;
        private readonly IWebHostEnvironment _env;

        public CardActionsController(CardService cardService, ActionResolver actionResolver, IWebHostEnvironment env)
        {
            _cardService = cardService;
            _actionResolver = actionResolver;
            _env = env;
        }

        [HttpGet("allowed-actions")]
        public async Task<IActionResult> GetAllowedActions([FromQuery] string userId, [FromQuery] string cardNumber)
        {
            var normalizedUserId = StringNormalizationHelper.NormalizeInput(userId);
            var normalizedCardNumber = StringNormalizationHelper.NormalizeInput(cardNumber);

            // Pobranie danych karty na podstawie userId i cardNumber.
            var card = await _cardService.GetCardDetails(normalizedUserId, normalizedCardNumber);
            if (card is null)
            {
                return NotFound(new { Message = "Card not found" });
            }

            string cardType = card.CardType.ToString();
            string cardStatus = card.CardStatus.ToString();

            var actions = _actionResolver.Resolve(cardType, cardStatus, card.IsPinSet);

            // W trybie Development dodajemy obiekt cardDetail do odpowiedzi
            if (_env.IsDevelopment())
            {
                return Ok(new
                {
                    CardDetail = card,
                    Actions = actions
                });
            }

            return Ok(new { Actions = actions });
        }
    }
}
