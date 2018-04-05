using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forte.SmokeTester.Extractor
{
    class CompositeExtractor : ILinkExtractor
    {
        private readonly IEnumerable<ILinkExtractor> extractors;

        public CompositeExtractor(params ILinkExtractor[] extractors)
        {
            this.extractors = extractors;
        }

        public async Task<IEnumerable<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content)
        {
            var result = new List<Uri>();
            foreach (var extractor in extractors)
            {
                result.AddRange(await extractor.ExtractLinks(crawlRequest, content));

            }

            return result;
        }
    }
}
