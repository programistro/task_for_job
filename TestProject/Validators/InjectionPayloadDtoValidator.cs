using FluentValidation;
using TestProject.Dtos;

namespace TestProject.Validators;

public class InjectionPayloadDtoValidator : AbstractValidator<InjectionPayloadDto>
{
    public InjectionPayloadDtoValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .MaximumLength(512);

        RuleFor(x => x.Attribute)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.UrlB64)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("'{PropertyName}' должен содержать корректный Base64.");

        RuleFor(x => x.EncryptedTextBytesB64)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("'{PropertyName}' должен содержать корректный Base64.");

        RuleFor(x => x.KeyBytesB64)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("'{PropertyName}' должен содержать корректный Base64.")
            .Must(x => x.Length > 0)
            .WithMessage("'{PropertyName}' не должен быть пустым после декодирования.");

        RuleFor(x => x.PageB64)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("'{PropertyName}' должен содержать корректный Base64.");
    }

    private static bool BeValidBase64(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        try
        {
            return Convert.FromBase64String(value).Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
