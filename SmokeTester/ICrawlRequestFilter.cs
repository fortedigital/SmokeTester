namespace Forte.SmokeTester
{
    public interface ICrawlRequestFilter
    {
        bool ShouldCrawl(CrawlRequest crawlRequest);
    }
}