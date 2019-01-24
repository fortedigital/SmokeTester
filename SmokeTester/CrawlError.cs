using System;
using System.Net;

namespace Forte.SmokeTester
{
    public class CrawlError
    {
        public readonly Uri Url;
        public readonly HttpStatusCode Status;
        public readonly Uri Referrer;
        public readonly Exception Exception;
        
        public CrawlError(Uri url, HttpStatusCode status, Uri referrer)
        {
            this.Url = url;
            this.Status = status;
            this.Referrer = referrer;
        }

        public CrawlError(Uri url, Exception exception, Uri referrer)
        {
            this.Url = url;
            this.Referrer = referrer;
            this.Exception = exception;
        }
    }
}