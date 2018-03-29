using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public class Crawler
    {
        private bool HaveMoreWork => this.workerPool.ActiveWorkersCount > 0 || this.workQueue.Count > 0;

        private readonly ILinkExtractor linkExtractor;
        private readonly ICrawlRequestFilter crawlRequestFilter;
        private readonly ICrawlerObserver observer;

        private readonly WorkerPool workerPool;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly BlockingCollection<CrawlRequest> workQueue = new BlockingCollection<CrawlRequest>(new ConcurrentQueue<CrawlRequest>());
        private readonly ConcurrentDictionary<Uri, CrawledUrlPropertiesImpl> discoveredUrls = new ConcurrentDictionary<Uri, CrawledUrlPropertiesImpl>();
        private readonly ISitemapLinkExtractor sitemapLinkExtractor;
        private readonly CrawlMode crawlMode;

        public Crawler(WorkerPool workerPool, ICrawlRequestFilter crawlRequestFilter, ILinkExtractor linkExtractor, ICrawlerObserver observer, int maxWorkers = 3)
        {
            this.crawlRequestFilter = crawlRequestFilter;
            this.linkExtractor = linkExtractor;
            this.observer = observer;
            this.workerPool = workerPool;
            this.crawlMode = CrawlMode.Crawl;
        }

        public Crawler(WorkerPool workerPool, ICrawlerObserver observer, ISitemapLinkExtractor sitemapLinkExtractor, int maxWorkers = 3)
        {
            this.observer = observer;
            this.workerPool = workerPool;
            this.sitemapLinkExtractor = sitemapLinkExtractor;
            this.crawlMode = CrawlMode.Sitemap;
        }

        public void SetEntryUrl(Uri url)
        {
            switch (this.crawlMode)
            {
                case CrawlMode.Crawl: Enqueue(url);
                    break;
                case CrawlMode.Sitemap:
                    var links = this.sitemapLinkExtractor.ExtractLinks(url, this.httpClient).Result;
                    foreach (var link in links)
                    {
                        Enqueue(link);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(crawlMode), this.crawlMode.ToString());
            }
        }

        protected void Enqueue(Uri url)
        {
            this.workQueue.Add(new CrawlRequest(url));
            this.discoveredUrls.TryAdd(url, new CrawledUrlPropertiesImpl(url));
        }

        public async Task<IReadOnlyDictionary<Uri, CrawledUrlProperties>> Crawl(CancellationToken cancellationToken = default(CancellationToken))
        {            
            await Task.Run(() =>
            {
                try
                {
                    while (this.HaveMoreWork && cancellationToken.IsCancellationRequested == false)
                    {
                        if (this.workQueue.TryTake(out var request, 1000, cancellationToken) == false)
                            continue;
                
                        this.workerPool.Run(async () =>
                        {
                            await this.Crawl(request, cancellationToken);
                        });
                    }

                }
                catch (TaskCanceledException)
                {
                }
            });

            return this.discoveredUrls.ToDictionary(kvp => kvp.Key, kvp => (CrawledUrlProperties)kvp.Value);
        }

        private async Task Crawl(CrawlRequest request, CancellationToken cancellationToken)
        {
            this.observer.OnCrawling(request);
            
            try
            {
                using (var response = await this.httpClient.GetAsync(request.Url, cancellationToken))
                {
                    this.discoveredUrls[request.Url].status = response.StatusCode;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        if (this.crawlMode == CrawlMode.Crawl)
                        {
                            var links = await this.linkExtractor.ExtractLinks(request, response.Content);
                            foreach (var url in links)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                    break;

                                if (this.ProcessExtractedUrl(request, url))
                                {
                                    var crawlRequest = new CrawlRequest(url, request.Url, request.Depth + 1);

                                    if (this.crawlRequestFilter.ShouldCrawl(crawlRequest) == false)
                                        continue;

                                    this.workQueue.Add(crawlRequest, cancellationToken);                                    
                                }
                            }                            
                        }
                    }
                    else
                    {
                        this.observer.OnError(new CrawlError(request.Url, response.StatusCode, request.Referer));
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                this.observer.OnError(new CrawlError(request.Url, e, request.Referer));
            }
        }

        private bool ProcessExtractedUrl(CrawlRequest request, Uri url)
        {
            var newUrl = false;
            if (this.discoveredUrls.TryGetValue(url, out var urlProperties) == false)
            {
                urlProperties = new CrawledUrlPropertiesImpl(url);
                if (this.discoveredUrls.TryAdd(url, urlProperties))
                {
                    newUrl = true;
                    this.observer.OnNewUrl(url);
                }
                else
                {
                    urlProperties = this.discoveredUrls[url];
                }
            }

            urlProperties.referers.TryAdd(request.Url, 0);
            
            return newUrl;
        }

        private class CrawledUrlPropertiesImpl : CrawledUrlProperties
        {
            public override HttpStatusCode? Status => this.status;
            public override IEnumerable<Uri> Referers => this.referers.Keys;

            public HttpStatusCode? status;
            public readonly ConcurrentDictionary<Uri, int> referers = new ConcurrentDictionary<Uri, int>();

            public CrawledUrlPropertiesImpl(Uri url) : base(url)
            {
            }
        }

    }
}