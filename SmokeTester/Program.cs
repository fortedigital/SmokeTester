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
            crawler.Enqueue(opts.StartUrls.Select(x => new Uri(x)));

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
            var startUrlAuthorities = opts.StartUrls.Select(x => new Uri(x).Authority);

            var linkExtractor = new DefaultLinkExtractor();
            var crawlRequestFilter = new CompositeFilter(
                new AuthorityFilter(startUrlAuthorities),
                new MaxDepthFilter(opts.MaxDepth));

            return new Crawler(
                new WorkerPool(opts.NumberOfWorkers),
                crawlRequestFilter,
                linkExtractor,
                observer,
                opts.RequestHeaders);
        }

        private static void WriteSummary(IReadOnlyDictionary<Uri, CrawledUrlProperties> result, CrawlerObserver observer)
        {
            Console.WriteLine($"\nDiscovered urls: {result.Count}\nCrawled urls: {observer.CrawledUrls.Count}\nCrawl warnings: {observer.Warnings.Count}\nCrawl errors: {observer.Errors.Count}");

            if (observer.Warnings.Count > 0)
            {
                Console.WriteLine("\nCrawl warnings:\n");
                foreach (var error in observer.Warnings)
                {
                    Console.WriteLine($"{error.Status}: {error.Url}\nReferrers:\n  {string.Join("\n  ", result[error.Url].Referrers)}\n");
                }
            }

            if (observer.Errors.Count > 0)
            {
                Console.WriteLine("\nCrawl errors:\n");
                foreach (var error in observer.Errors)
                {
                    Console.WriteLine($"{error.Exception?.FlattenInnerMessages() ?? error.Status.ToString()}: {error.Url}\nReferrers:\n  {string.Join("\n  ", result[error.Url].Referrers)}\n");
                }
            }
        }
    }
}
