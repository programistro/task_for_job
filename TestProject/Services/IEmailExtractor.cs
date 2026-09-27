using TestProject.Dtos;

namespace TestProject.Services;

public interface IEmailExtractor
{
    EmailExtractionResultDto Extract(string html);
}
