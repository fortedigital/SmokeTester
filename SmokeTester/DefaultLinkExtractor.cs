using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Html;
using AngleSharp.Parser.Html;

namespace Forte.SmokeTester
{
    public class DefaultLinkExtractor : ILinkExtractor
    {
        public async Task<IReadOnlyCollection<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content)
        {
            var mediaType = content.Headers.ContentType.MediaType;

            var isHtml = string.Equals(mediaType, "text/html", StringComparison.OrdinalIgnoreCase);
            if (isHtml)
            {
                using (var contentStream = await content.ReadAsStreamAsync())
                {
                    var parser = new HtmlParser();
                    var document = await parser.ParseAsync(contentStream);

                    return document.Links
                        .Select(l => l.GetAttribute(AttributeNames.Href))
                        .Select(href => BuildUri(crawlRequest, href))
                        .Where(uri => uri.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase) == false)
                        .ToList();
                }
            }

            // try to parse it as site map
            var isXml= string.Equals(mediaType, "text/xml", StringComparison.OrdinalIgnoreCase);
            if (isXml)
            {
                var document = XDocument.Parse(await content.ReadAsStringAsync());
                if (document.Root != null)
                {
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("x", document.Root.Name.Namespace.ToString());

                    return document.Root.XPathSelectElements("//x:loc", namespaceManager)
                        .Select(x => x.Value.Trim())
                        .Select(x => BuildUri(crawlRequest, x))
                        .ToList();
                }
            }

            return new Uri[0];


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