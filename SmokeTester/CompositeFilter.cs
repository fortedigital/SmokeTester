using System.Collections.Generic;
using System.Linq;

namespace Forte.SmokeTester
{
    public class CompositeFilter : ICrawlRequestFilter
    {
        private readonly IEnumerable<ICrawlRequestFilter> filters;

        public CompositeFilter(params ICrawlRequestFilter[] filters)
        {
            this.filters = filters;
        }

        public bool ShouldCrawl(CrawlRequest crawlRequest)
        {
            return this.filters.All(f => f.ShouldCrawl(crawlRequest));
        }
    }
}