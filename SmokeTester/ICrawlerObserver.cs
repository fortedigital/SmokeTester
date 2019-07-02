using System;

namespace Forte.SmokeTester
{
    public interface ICrawlerObserver
    {
        void OnRetrying(CrawlError error);
        void OnError(CrawlError error);
        void OnCrawling(CrawlRequest request);
        void OnNewUrl(Uri url);
        void OnCrawled(CrawlResult result);
    }
}