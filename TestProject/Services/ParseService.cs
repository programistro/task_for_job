using System.Security.Cryptography;
using System.Text;
using AngleSharp;
using TestProject.Dtos;
using TestProject.Repositories;

namespace TestProject.Services;

public class ParseService : IParseService
{
    private readonly ILogger<ParseService> _logger;
    private readonly IEmailExtractor _emailExtractor;
    private readonly IEncryptionService _encryptionService;
    private readonly IElementRepository _elementRepository;

    public ParseService(
        ILogger<ParseService> logger,
        IEmailExtractor emailExtractor,
        IEncryptionService encryptionService,
        IElementRepository elementRepository)
    {
        _logger = logger;
        _emailExtractor = emailExtractor;
        _encryptionService = encryptionService;
        _elementRepository = elementRepository;
    }

    public async Task<ParseResultDto> Parse(InjectionPayloadDto dto, CancellationToken cancellationToken = default)
    {
        var pageHtml = Encoding.UTF8.GetString(Convert.FromBase64String(dto.PageB64));
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(pageHtml), cancellationToken);

        var matched = document.QuerySelectorAll(dto.Selector);
        _logger.LogInformation("Selector {Selector} matched {Count} element(s).", dto.Selector, matched.Length);

        var elements = new List<ParsedElementDto>(matched.Length);
        var values = new List<string>(matched.Length);
        for (var i = 0; i < matched.Length; i++)
        {
            var value = matched[i].GetAttribute(dto.Attribute);
            if (value is null)
            {
                continue;
            }

            elements.Add(new ParsedElementDto
            {
                Index = i,
                Tag = matched[i].LocalName,
                Attribute = dto.Attribute,
                Value = value
            });
            values.Add(value);
            _logger.LogInformation("Element {Index} <{Tag}> {Attribute}={Value}", i, matched[i].LocalName, dto.Attribute, Sanitize(value));
        }

        _logger.LogInformation("Collected {Collected} value(s) of attribute {Attribute} from {Matched} element(s).", values.Count, dto.Attribute, matched.Length);

        var elementIds = new List<long>(values.Count);
        foreach (var value in values)
        {
            var id = await _elementRepository.AddAsync(value, dto.Attribute, cancellationToken);
            elementIds.Add(id);
        }

        _logger.LogInformation("Persisted {Count} element(s) to the elements table.", elementIds.Count);

        var emails = _emailExtractor.Extract(pageHtml);
        _logger.LogInformation("Found {Count} email(s).", emails.Count);
        for (var i = 0; i < emails.Emails.Count; i++)
        {
            _logger.LogInformation("Email {Index}: {Email}", i, emails.Emails[i]);
        }

        string? decryptedText = null;
        string? decryptionError = null;
        try
        {
            decryptedText = _encryptionService.Decrypt(dto.EncryptedTextBytesB64, dto.KeyBytesB64);
            _logger.LogInformation("Decrypted text: {Text}", Sanitize(decryptedText));
        }
        catch (Exception ex) when (ex is CryptographicException or ArgumentException or FormatException)
        {
            decryptionError = ex.Message;
            _logger.LogWarning(ex, "Failed to decrypt encrypted_text_bytes_b64: {Message}", ex.Message);
        }

        return new ParseResultDto
        {
            Selector = dto.Selector,
            MatchedCount = matched.Length,
            Elements = elements,
            CollectedCount = values.Count,
            ElementIds = elementIds,
            Values = values,
            EmailCount = emails.Count,
            Emails = emails.Emails,
            DecryptedText = decryptedText,
            DecryptionError = decryptionError
        };
    }

    private static string Sanitize(string value)
    {
        return value.ReplaceLineEndings("\\n");
    }
}
