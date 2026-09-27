using System.Text.Json.Serialization;

namespace TestProject.Dtos;

public class ParsedElementDto
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("tag")]
    public string Tag { get; init; } = string.Empty;

    [JsonPropertyName("attribute")]
    public string Attribute { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;
}
