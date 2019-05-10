using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RobotsTxt;

namespace Forte.SmokeTester.Extractor
{
    public class RobotsTxtSitemapExtractor : ILinkExtractor
    {
        public async Task<IReadOnlyCollection<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content)
        {
            if (crawlRequest.Url.PathAndQuery.Equals("/robots.txt", StringComparison.OrdinalIgnoreCase) == false)
                return new Uri[0];

            var contentString = await content.ReadAsStringAsync();
            try
            {
                var robots = Robots.Load(contentString);
                return robots.Sitemaps.Select(s => s.Url).ToList();
            }
            catch
            {
                 return new Uri[0];
            }
        }
    }
}
