using System.Text.RegularExpressions;
using System.Text;
using SaleApi.Models.Extended;
using Common.Utilities;
namespace SaleApi.Models.Services
{
    public class UrlHelper
    {
        public static string ToFriendlyUrl(string title)
        {

            title = Utility.ConvertToUnsign(title);
            // Make the string lowercase.
            title = title.ToLowerInvariant();

            // Remove all accents.
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(title);
            title = Encoding.ASCII.GetString(bytes);

            // Replace spaces.
            title = Regex.Replace(title, @"\s", "-", RegexOptions.Compiled);

            // Remove invalid characters.
            title = Regex.Replace(title, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            // Trim dashes from the beginning and end.
            title = title.Trim('-', '_');

            // Replace multiple dashes or whitespaces.
            title = Regex.Replace(title, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return title;
        }
    }
}
