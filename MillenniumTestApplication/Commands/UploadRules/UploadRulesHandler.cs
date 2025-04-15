using MillenniumTestApplication.Domain.Interfaces;
using MillenniumTestApplication.Domain.Rules;
using MillenniumTestApplication.Infrastructure.Parsers;
using MillenniumTestApplication.Shared.Helpers;
using System.Text.Json;
using MediatR;

namespace MillenniumTestApplication.Commands.UploadRules
{
    public class UploadRulesHandler : IRequestHandler<UploadRulesCommand>
    {
        private readonly IActionResolver _resolver;

        public UploadRulesHandler(IActionResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task Handle(UploadRulesCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                throw new InvalidDataException("Plik jest pusty lub nie został przesłany.");

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);

            await using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            List<ActionRule> parsedRules;

            if (extension == ".csv")
            {
                parsedRules = CsvToJsonRuleConverter.ConvertToMemory(tempPath);
            }
            else if (extension == ".json")
            {
                var json = await File.ReadAllTextAsync(tempPath, cancellationToken);
                parsedRules = JsonSerializer.Deserialize<List<ActionRule>>(json, JsonOptionsHelper.CaseInsensitive)
                    ?? throw new InvalidDataException("Niepoprawna struktura pliku JSON.");
            }
            else
            {
                throw new InvalidDataException("Dozwolone formaty to .csv i .json.");
            }

            if (parsedRules == null || parsedRules.Count == 0)
            {
                throw new InvalidDataException("Brak poprawnych reguł w pliku.");
            }

            _resolver.UpdateRules(parsedRules);
        }
    }
}
