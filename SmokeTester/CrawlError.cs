using System;
using System.Net;

namespace Forte.SmokeTester
{
    public class CrawlError
    {
        public readonly Uri Url;
        public readonly HttpStatusCode Status;
        public readonly Uri Referer;
        public readonly Exception Exception;
        
        public CrawlError(Uri url, HttpStatusCode status, Uri referer)
        {
            Url = url;
            Status = status;
            Referer = referer;
        }

        public CrawlError(Uri url, Exception exception, Uri referer)
        {
            this.Url = url;
            this.Referer = referer;
            this.Exception = exception;
        }
    }
}