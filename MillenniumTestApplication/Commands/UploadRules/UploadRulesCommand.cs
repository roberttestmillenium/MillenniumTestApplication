using MediatR;

namespace MillenniumTestApplication.Commands.UploadRules
{
    public class UploadRulesCommand : IRequest
    {
        public IFormFile File { get; }

        public UploadRulesCommand(IFormFile file)
        {
            File = file;
        }
    }
}
