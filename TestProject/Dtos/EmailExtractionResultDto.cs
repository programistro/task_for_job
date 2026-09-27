using System.Text.Json.Serialization;

namespace TestProject.Dtos;

public class EmailExtractionResultDto
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("emails")]
    public IReadOnlyList<string> Emails { get; init; } = [];
}
