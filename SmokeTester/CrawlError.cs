using System;
using System.Net;

namespace Forte.SmokeTester
{
    public class CrawlError
    {
        public Uri Url { get; }
        public HttpStatusCode Status { get; set; }
        public Uri Referer { get; }

        public CrawlError(Uri url, HttpStatusCode status, Uri referer)
        {
            Url = url;
            Status = status;
            Referer = referer;
        }
    }
}