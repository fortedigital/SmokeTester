using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Forte.SmokeTester
{
    public class SitemapLinkExtractor : ISitemapLinkExtractor
    {
        
        public async Task<IEnumerable<Uri>> ExtractLinks(Uri sitemapUri, HttpClient httpClient)
        {
            var result = new List<Uri>();
            using (var response = await httpClient.GetAsync(sitemapUri))
            using (var xmlReader = XmlReader.Create(await response.Content.ReadAsStreamAsync()))
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