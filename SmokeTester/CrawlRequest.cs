using System;

namespace Forte.SmokeTester
{
    public class CrawlRequest
    {
        public readonly Uri Url;
        public readonly Uri Referer;
        public readonly int Depth;
            
        public CrawlRequest(Uri url, Uri referer = null, int depth = 0)
        {
            this.Url = url;
            this.Referer = referer;
            this.Depth = depth;
        }
    }
}