using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Html;

namespace MAOToolkit.Extensions
{
    public static class HtmlContentExtensions
    {
        private static readonly HtmlEncoder UnicodeHtmlEncoder = HtmlEncoder.Create(UnicodeRanges.All);
        
        public static string GetString(this IHtmlContent content)
        {
            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, UnicodeHtmlEncoder);
                return writer.ToString();
            }
        }
    }
}