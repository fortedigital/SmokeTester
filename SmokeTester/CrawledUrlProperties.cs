using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Forte.SmokeTester
{
    public abstract class CrawledUrlProperties
    {
        public readonly Uri Url;
        public abstract HttpStatusCode? Status { get; }
        public abstract IEnumerable<Uri> Referrers { get; }

        protected CrawledUrlProperties(Uri url)
        {
            this.Url = url;
        }
    }
}