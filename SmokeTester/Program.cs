using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace Forte.SmokeTester
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return CommandLine.Parser.Default
                .ParseArguments<CrawlOptions, SitemapOptions>(args)
                .MapResult(
                    (CrawlOptions crawlOptions) => Run(crawlOptions),
                    (SitemapOptions sitemapOptions) => Run(sitemapOptions),
                 errs => 1);
        }

        private static int Run(CommonOptions opts)
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var observer = new CrawlerObserver(
                cancelationTokenSource, 
                opts.MaxErrors, 
                opts.MaxUrls);          
            
            var crawler = CreateCrwaler(opts, observer);
            crawler.SetEntryUrl(new Uri(opts.GetStartUrl()));                
            
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancelationTokenSource.Cancel();
            };

            var result = crawler.Crawl(cancelationTokenSource.Token).Result;
            
            WriteSummary(result, observer);

            return observer.Errors.Count > 0 ? 1 : 0;
        }

        private static Crawler CreateCrwaler(CommonOptions commonOptions, ICrawlerObserver observer)
        {
            var workerPool = new WorkerPool(commonOptions.NumberOfWorkers);
            if (commonOptions is CrawlOptions opts)
            {
                var startUrl = new Uri(opts.StartUrl);

                var linkExtractor = new DefaultLinkExtractor();
                var crawlRequestFilter = new CompositeFilter(
                    new AuthorityFilter(startUrl.Authority),
                    new MaxDepthFilter(opts.MaxDepth));
                
                return new Crawler(
                    workerPool, 
                    crawlRequestFilter,
                    linkExtractor,
                    observer);   
            }

            if (commonOptions is SitemapOptions)
            {
                return new Crawler(workerPool, observer, new SitemapLinkExtractor());
            }
            
            throw new ArgumentException($"Not supported options type: {commonOptions.GetType()}");

        }

        private static void WriteSummary(IReadOnlyDictionary<Uri, CrawledUrlProperties> result, CrawlerObserver observer)
        {
            Console.WriteLine($"\nDiscovered urls: {result.Count}\nCrawled urls: {observer.CrawledUrls.Count}\nCrawl warnings: {observer.Warnings.Count}\nCrawl errors: {observer.Errors.Count}");

            if (observer.Warnings.Count > 0)
            {
                Console.WriteLine("\nCrawl warnings:\n");
                foreach (var error in observer.Warnings)
                {
                    Console.WriteLine($"{error.Status}: {error.Url}");
                    if (result[error.Url].Referers.Any())
                    {
                        Console.WriteLine($"Referers:\n  {string.Join("\n ", result[error.Url].Referers)}\n");
                    }
                }
            }

            if (observer.Errors.Count > 0)
            {
                Console.WriteLine("\nCrawl errors:\n");
                foreach (var error in observer.Errors)
                {
                    Console.WriteLine($"{error.Exception?.Message ?? error.Status.ToString()}: {error.Url}");
                    if (result[error.Url].Referers.Any())
                    {
                        Console.WriteLine($"Referers:\n  {string.Join("\n  ", result[error.Url].Referers)}\n");
                    }
                }
            }
        }
    }
}
