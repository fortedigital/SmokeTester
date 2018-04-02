using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public interface ISitemapLinkExtractor
    {
        Task<IEnumerable<Uri>> ExtractLinks(Uri sitemapUri, HttpClient httpClient);
    }
}