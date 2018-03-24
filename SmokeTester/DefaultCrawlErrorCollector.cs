using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public class DefaultCrawlErrorCollector : ICrawlErrorCollector
    {
        public readonly ConcurrentBag<CrawlError> Errors = new ConcurrentBag<CrawlError>();
        public readonly ConcurrentBag<CrawlError> Warnings = new ConcurrentBag<CrawlError>();
        
        public async Task CollectError(CrawlError error)
        {
            if (error.Status == HttpStatusCode.NotFound)
                this.Warnings.Add(error);
            else
                this.Errors.Add(error);
        }
    }
}