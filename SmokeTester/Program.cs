namespace Forte.SmokeTester
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return 0;
//            return CommandLine.Parser.Default
//                .ParseArguments<Options>(args)
//                .MapResult(Run, errs => 1);
        }

//        private static int Run(Options opts)
//        {
//            var cancellationTokenSource = new CancellationTokenSource();
//            var observer = new CrawlerObserver(
//                cancellationTokenSource,
//                opts.MaxErrors,
//                opts.MaxUrls);
//
//            var crawler = CreateCrawler(opts, observer);
//            crawler.Enqueue(opts.StartUrls.Select(x => new Uri(x)));
//
//            var parseRobots = opts.NoRobots == false;
//            if (parseRobots)
//            {
//                var rootUrls = opts.StartUrls.Select(x => new Uri(x).GetLeftPart(UriPartial.Authority));
//                var robotsTxtUrls = rootUrls.Select(x => new Uri(new Uri(x), "/robots.txt")).Distinct();
//                crawler.Enqueue(robotsTxtUrls);
//            }
//
//            Console.CancelKeyPress += (sender, eventArgs) =>
//            {
//                eventArgs.Cancel = true;
//                cancellationTokenSource.Cancel();
//            };
//
//            var result = crawler.Crawl(cancellationTokenSource.Token).Result;
//
//            WriteSummary(result, observer, opts.FullSummary);
//            
//            var crawledUrlsCount = observer.CrawledUrls.Count;
//            if (crawledUrlsCount < opts.MinUrls)
//            {
//                Console.WriteLine($"\nExpected at least {opts.MinUrls} urls but crawled only {crawledUrlsCount}.");
//                return 2;
//            }
//
//            return observer.Errors.Count > 0 ? 1 : 0;
//        }
//
//        private static Crawler CreateCrawler(Options opts, ICrawlerObserver observer)
//        {
//            var startUrlAuthorities = opts.StartUrls.Select(x => new Uri(x).Authority);
//
//            var linkExtractor = new CompositeExtractor(new HtmlLinkExtractor(), new SiteMapLinkExtractor(), new RobotsTxtSitemapExtractor());
//            var crawlRequestFilter = new CompositeFilter(
//                new AuthorityFilter(startUrlAuthorities, opts.TestExternalUrls),
//                new MaxDepthFilter(opts.MaxDepth));
//
//            var requestTimeout = opts.RequestTimeout != null
//                ? (TimeSpan?)TimeSpan.FromSeconds(opts.RequestTimeout.Value)
//                : null;
//
//            return new Crawler(
//                new WorkerPool(opts.NumberOfWorkers),
//                crawlRequestFilter,
//                linkExtractor,
//                observer,
//                opts.RequestHeaders,
//                requestTimeout,
//                opts.MaxRetries,
//                opts.UserAgent);
//        }
//
//        private static void WriteSummary(IReadOnlyDictionary<Uri, CrawledUrlProperties> result, CrawlerObserver observer, bool fullSummary)
//        {
//            Console.WriteLine($"\nDiscovered urls: {result.Count}\nCrawled urls: {observer.CrawledUrls.Count}\nCrawl warnings: {observer.Warnings.Count}\nCrawl errors: {observer.Errors.Count}");
//
//            if (observer.Warnings.Count > 0)
//            {
//                Console.WriteLine("\nCrawl warnings:\n");
//                foreach (var error in observer.Warnings)
//                {
//                    Console.WriteLine($"{error.Status}: {error.Url}\nReferrers:\n  {string.Join("\n  ", result[error.Url].Referrers)}\n");
//                }
//            }
//
//            if (observer.Errors.Count > 0)
//            {
//                Console.WriteLine("\nCrawl errors:\n");
//                foreach (var error in observer.Errors)
//                {
//                    Console.WriteLine($"{error.Exception?.FlattenInnerMessages() ?? error.Status.ToString()}: {error.Url}\nReferrers:\n  {string.Join("\n  ", result[error.Url].Referrers)}\n");
//                }
//            }
//
//            if (fullSummary)
//            {
//                Console.WriteLine("###########################################################################################33");
//                foreach (var uri in result.Where(x => x.Value.Status != null).Select(x => x.Key).OrderBy(x => x.ToString()))
//                {
//                    var entry = result[uri];
//                    Console.WriteLine($"[{entry.Status}] {uri}: \nReferrers:\n  {string.Join("\n  ", entry.Referrers)}\n");
//                }
//            }
//        }
    }
}
