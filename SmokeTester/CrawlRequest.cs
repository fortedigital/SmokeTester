using System;

namespace Forte.SmokeTester
{
    public class CrawlRequest
    {
        public readonly Uri Url;
        public readonly Uri Referrer;
        public readonly int Depth;
            
        public CrawlRequest(Uri url, Uri referrer = null, int depth = 0)
        {
            this.Url = url;
            this.Referrer = referrer;
            this.Depth = depth;
        }
    }
}