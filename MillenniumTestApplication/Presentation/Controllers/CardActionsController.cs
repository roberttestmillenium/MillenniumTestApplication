using MediatR;
using Microsoft.AspNetCore.Mvc;
using MillenniumTestApplication.Application.Queries.GetAllowedActions;
using MillenniumTestApplication.Commands.UploadRules;

namespace MillenniumTestApplication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardActionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public CardActionsController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        [HttpGet("allowed-actions")]
        public async Task<IActionResult> Get([FromQuery] string userId, [FromQuery] string cardNumber)
        {
            try
            {
                var result = await _mediator.Send(new GetAllowedActionsQuery(userId, cardNumber));
                if (_env.IsDevelopment())
                    return Ok(new { result.Card, result.Actions });

                return Ok(new { result.Actions });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost("upload-rules")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nie przesłano pliku.");

            await _mediator.Send(new UploadRulesCommand(file));
            return Ok("Reguły zostały zaktualizowane w pamięci.");
        }
    }
}
