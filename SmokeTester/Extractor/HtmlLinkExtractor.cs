using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html;
using AngleSharp.Parser.Html;

namespace Forte.SmokeTester.Extractor
{
    public class HtmlLinkExtractor : ILinkExtractor
    {
        public async Task<IEnumerable<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content)
        {
            if ("text/html".Equals(content.Headers.ContentType.MediaType, StringComparison.OrdinalIgnoreCase) == false)
                return Enumerable.Empty<Uri>();

            using (var contentStream = await content.ReadAsStreamAsync())
            {
                var parser = new HtmlParser();
                var document = await parser.ParseAsync(contentStream);

                return document.Links
                    .Select(l => l.GetAttribute(AttributeNames.Href))
                    .Select(href => BuildUri(crawlRequest, href))
                    .Where(uri => uri.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase) == false);
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
    }
}
