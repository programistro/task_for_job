using System.Text.Json.Serialization;

namespace TestProject.Dtos;

public class ParseResultDto
{
    [JsonPropertyName("selector")]
    public string Selector { get; init; } = string.Empty;

    [JsonPropertyName("matched_count")]
    public int MatchedCount { get; init; }

    [JsonPropertyName("elements")]
    public IReadOnlyList<ParsedElementDto> Elements { get; init; } = [];

    [JsonPropertyName("collected_count")]
    public int CollectedCount { get; init; }

    [JsonPropertyName("element_ids")]
    public IReadOnlyList<long> ElementIds { get; init; } = [];

    [JsonPropertyName("values")]
    public IReadOnlyList<string> Values { get; init; } = [];

    [JsonPropertyName("email_count")]
    public int EmailCount { get; init; }

    [JsonPropertyName("emails")]
    public IReadOnlyList<string> Emails { get; init; } = [];

    [JsonPropertyName("decrypted_text")]
    public string? DecryptedText { get; init; }

    [JsonPropertyName("decryption_error")]
    public string? DecryptionError { get; init; }
}
