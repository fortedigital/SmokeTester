using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Forte.SmokeTester
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return CommandLine.Parser.Default
                .ParseArguments<Options>(args)
                .MapResult(Run, errs => 1);
        }

        private static int Run(Options opts)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var observer = new CrawlerObserver(
                cancellationTokenSource,
                opts.MaxErrors,
                opts.MaxUrls);

            var crawler = CreateCrawler(opts, observer);
            crawler.Enqueue(new Uri(opts.StartUrl));

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            var result = crawler.Crawl(cancellationTokenSource.Token).Result;

            WriteSummary(result, observer);

            return observer.Errors.Count > 0 ? 1 : 0;
        }

        private static Crawler CreateCrawler(Options opts, ICrawlerObserver observer)
        {
            var startUrl = new Uri(opts.StartUrl);

            var linkExtractor = new DefaultLinkExtractor();
            var crawlRequestFilter = new CompositeFilter(
                new AuthorityFilter(startUrl.Authority),
                new MaxDepthFilter(opts.MaxDepth));

            var customHttpHeaders = (opts.RequestHeaders ?? "")
                .Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().Split(':', 2))
                .ToDictionary(x => x.ElementAt(0), x => x.ElementAtOrDefault(1));

            return new Crawler(
                new WorkerPool(opts.NumberOfWorkers),
                crawlRequestFilter,
                linkExtractor,
                observer,
                customHttpHeaders);
        }

        private static void WriteSummary(IReadOnlyDictionary<Uri, CrawledUrlProperties> result, CrawlerObserver observer)
        {
            Console.WriteLine($"\nDiscovered urls: {result.Count}\nCrawled urls: {observer.CrawledUrls.Count}\nCrawl warnings: {observer.Warnings.Count}\nCrawl errors: {observer.Errors.Count}");

            if (observer.Warnings.Count > 0)
            {
                Console.WriteLine("\nCrawl warnings:\n");
                foreach (var error in observer.Warnings)
                {
                    Console.WriteLine($"{error.Status}: {error.Url}\nReferrers:\n  {string.Join("\n  ", result[error.Url].Referers)}\n");
                }
            }

            if (observer.Errors.Count > 0)
            {
                Console.WriteLine("\nCrawl errors:\n");
                foreach (var error in observer.Errors)
                {
                    Console.WriteLine($"{error.Exception?.Message ?? error.Status.ToString()}: {error.Url}\nReferers:\n  {string.Join("\n  ", result[error.Url].Referers)}\n");
                }
            }
        }
    }
}
