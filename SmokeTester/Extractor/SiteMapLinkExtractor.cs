using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Forte.SmokeTester.Extractor
{
    class SiteMapLinkExtractor : ILinkExtractor
    {
        public async Task<IReadOnlyCollection<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content)
        {
            if ("text/xml".Equals(content.Headers.ContentType?.MediaType, StringComparison.OrdinalIgnoreCase) == false)
                return new Uri[0];


            var result = new List<Uri>();
            using (var xmlReader = XmlReader.Create(await content.ReadAsStreamAsync()))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "loc")
                    {
                        var el = (XElement) XNode.ReadFrom(xmlReader);
                        
                        result.Add(new Uri(el.Value));
                    }
                }
            }

            return result;
        }
    }
}
