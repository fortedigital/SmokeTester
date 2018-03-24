using System;
using System.Collections.Generic;

namespace Forte.SmokeTester
{
    public class CrawlResult
    {
        public readonly IReadOnlyDictionary<Uri, IReadOnlyCollection<Uri>> DiscoveredUrls;
        public readonly int CrawledUrlsCount;

        public CrawlResult(int crawledUrlsCount, IReadOnlyDictionary<Uri, IReadOnlyCollection<Uri>> discoveredUrls)
        {
            this.DiscoveredUrls = discoveredUrls;
            this.CrawledUrlsCount = crawledUrlsCount;
        }
    }
}