namespace Forte.SmokeTester
{
    public class MaxDepthFilter : ICrawlRequestFilter
    {
        private readonly int maxDepth;

        public MaxDepthFilter(int maxDepth)
        {
            this.maxDepth = maxDepth;
        }

        public bool ShouldCrawl(CrawlRequest crawlRequest)
        {
            return crawlRequest.Depth <= this.maxDepth;
        }
    }
}