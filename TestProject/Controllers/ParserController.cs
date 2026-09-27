using Microsoft.AspNetCore.Mvc;
using TestProject.Dtos;
using TestProject.Services;
using TestProject.Validators;

namespace TestProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ParserController : ControllerBase
{
    private readonly InjectionPayloadDtoValidator _validator;
    private readonly IParseService _parseService;

    public ParserController(InjectionPayloadDtoValidator validator, IParseService parseService)
    {
        _validator = validator;
        _parseService = parseService;
    }

    [HttpPost]
    public async Task<IActionResult> Parse([FromBody] InjectionPayloadDto injectionPayloadDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(injectionPayloadDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var result = await _parseService.Parse(injectionPayloadDto, cancellationToken);

        return Ok(result);
    }
}
