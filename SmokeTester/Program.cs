using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Forte.SmokeTester.Extractor;

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
            var cancelationTokenSource = new CancellationTokenSource();
            var observer = new CrawlerObserver(
                cancelationTokenSource, 
                opts.MaxErrors, 
                opts.MaxUrls);          
            
            var crawler = CreateCrwaler(opts, observer);
            crawler.Enqueue(new Uri(opts.StartUrl));

            if (!opts.NoRobots)
            {
                var rootUrl = new Uri(opts.StartUrl).GetLeftPart(UriPartial.Authority);
                var robotsTxtUrl = new Uri(new Uri(rootUrl), "/robots.txt");
                crawler.Enqueue(robotsTxtUrl);                
            }
            
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancelationTokenSource.Cancel();
            };

            var result = crawler.Crawl(cancelationTokenSource.Token).Result;
            
            WriteSummary(result, observer);

            return observer.Errors.Count > 0 ? 1 : 0;
        }

        private static Crawler CreateCrwaler(Options opts, ICrawlerObserver observer)
        {
            var startUrl = new Uri(opts.StartUrl);

            var linkExtractor = new CompositeExtractor(new HtmlLinkExtractor(), new SiteMapLinkExtractor(), new RobotsTxtSitemapExtractor());
            var crawlRequestFilter = new CompositeFilter(
                new AuthorityFilter(startUrl.Authority),
                new MaxDepthFilter(opts.MaxDepth));

            return new Crawler(
                new WorkerPool(opts.NumberOfWorkers), 
                crawlRequestFilter,
                linkExtractor,
                observer);
        }

        private static void WriteSummary(IReadOnlyDictionary<Uri, CrawledUrlProperties> result, CrawlerObserver observer)
        {
            Console.WriteLine($"\nDiscovered urls: {result.Count}\nCrawled urls: {observer.CrawledUrls.Count}\nCrawl warnings: {observer.Warnings.Count}\nCrawl errors: {observer.Errors.Count}");

            if (observer.Warnings.Count > 0)
            {
                Console.WriteLine("\nCrawl warnings:\n");
                foreach (var error in observer.Warnings)
                {
                    Console.WriteLine($"{error.Status}: {error.Url}\nReferers:\n  {string.Join("\n  ", result[error.Url].Referers)}\n");
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
