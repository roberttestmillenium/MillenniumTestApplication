using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MillenniumTestApplication.Helpers;
using MillenniumTestApplication.Models;
using MillenniumTestApplication.Services;

namespace MillenniumTestApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RulesUploadController : ControllerBase
    {
        private readonly ActionResolver _resolver;

        public RulesUploadController(ActionResolver resolver)
        {
            _resolver = resolver;
        }

        [HttpPost]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nie przesłano pliku.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var tempPath = Path.GetTempFileName();

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                List<ActionRule> parsedRules;

                if (extension == ".csv")
                {
                    parsedRules = CsvToJsonRuleConverter.ConvertToMemory(tempPath);
                }
                else if (extension == ".json")
                {
                    var json = await System.IO.File.ReadAllTextAsync(tempPath);
                    parsedRules = JsonSerializer.Deserialize<List<ActionRule>>(json);

                    if (parsedRules == null || !parsedRules.Any())
                        return BadRequest("Niepoprawny format JSON.");
                }
                else
                {
                    return BadRequest("Obsługiwane formaty: .csv lub .json.");
                }

                _resolver.UpdateRules(parsedRules);
                return Ok("Reguły zostały załadowane do pamięci.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Błąd przetwarzania pliku: {ex.Message}");
            }
        }
    }
}
