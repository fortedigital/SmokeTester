using System;
using System.Collections.Generic;
using System.Net;

namespace Forte.SmokeTester
{
    public class CrawlResult
    {
        public Uri Url { get; }
        public HttpStatusCode Status { get; }
        public Uri Referrer { get; }
        public TimeSpan RequestDuration { get; }

        public CrawlResult(Uri url, HttpStatusCode status, Uri referrer, TimeSpan requestDuration)
        {
            this.Url = url;
            this.Status = status;
            this.Referrer = referrer;
            this.RequestDuration = requestDuration;
        }
    }
}