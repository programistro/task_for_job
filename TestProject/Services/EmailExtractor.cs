using System.Text.RegularExpressions;
using TestProject.Dtos;

namespace TestProject.Services;

public partial class EmailExtractor : IEmailExtractor
{
    public EmailExtractionResultDto Extract(string html)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var emails = new List<string>();

        foreach (Match match in EmailRegex().Matches(html))
        {
            if (seen.Add(match.Value))
            {
                emails.Add(match.Value);
            }
        }

        return new EmailExtractionResultDto { Count = emails.Count, Emails = emails };
    }

    [GeneratedRegex(@"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+(?:\.[a-zA-Z0-9\-]+)*\.[a-zA-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();
}
