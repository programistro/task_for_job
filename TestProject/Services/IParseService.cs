using TestProject.Dtos;

namespace TestProject.Services;

public interface IParseService
{
    Task<ParseResultDto> Parse(InjectionPayloadDto dto, CancellationToken cancellationToken = default);
}
