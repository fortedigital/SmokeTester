using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public interface ILinkExtractor
    {
        Task<IEnumerable<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content);
    }
}