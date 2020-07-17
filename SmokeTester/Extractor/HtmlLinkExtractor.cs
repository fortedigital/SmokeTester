using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Forte.SmokeTester.Extractor
{
    public class HtmlLinkExtractor : ILinkExtractor
    {
        private static readonly IEnumerable<string> exludedSchemas = new[]
        {
            "mailto",
            "tel",
            "script"
        };

        public async Task<IReadOnlyCollection<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content)
        {
            if ("text/html".Equals(content.Headers.ContentType.MediaType, StringComparison.OrdinalIgnoreCase) == false)
                return new Uri[0];

            using (var contentStream = await content.ReadAsStreamAsync())
            {
                var parser = new HtmlParser();
                var document = await parser.ParseDocumentAsync(contentStream);

                return document.Links
                    .Select(l => l.GetAttribute(AttributeNames.Href))
                    .Select(href => BuildUri(crawlRequest, href))
                    .Where(uri => exludedSchemas.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase) == false)
                    .Select(this.RemoveFragment)
                    .Distinct()
                    .ToList();
            }
        }

        private static Uri BuildUri(CrawlRequest crawlRequest, string href)
        {
            try
            {
                return new Uri(crawlRequest.Url, new Uri(href, UriKind.RelativeOrAbsolute));
            }
            catch (Exception e)
            {
                throw new FormatException($"Invalid URI format: '{href}'.", e);
            }
        }

        private Uri RemoveFragment(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Fragment))
            {
                return uri;
            }

            return new Uri( uri.GetLeftPart(UriPartial.Query),UriKind.RelativeOrAbsolute);
        }
    }
}
