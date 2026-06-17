using System.Text.RegularExpressions;

namespace KupujDomace.Services;


public static class Html
{
    // Mirrors strip_html_links() in farm_service.py / product_service.py:
    // remove <a> tags but keep their inner text.
    private static readonly Regex LinkPattern = new("<a[^>]*>(.*?)</a>", RegexOptions.Compiled);

    public static string StripLinks(string htmlContent) =>
        LinkPattern.Replace(htmlContent, "$1");
}
