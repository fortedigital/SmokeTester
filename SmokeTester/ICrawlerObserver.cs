using System;

namespace Forte.SmokeTester
{
    public interface ICrawlerObserver
    {
        void OnError(CrawlError error);
        void OnCrawling(CrawlRequest request);
        void OnNewUrl(Uri url);
    }
}