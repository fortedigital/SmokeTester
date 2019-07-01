using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Forte.SmokeTester.Extractor;

namespace Forte.SmokeTester
{
    public class Crawler
    {
        private bool HaveMoreWork => this.workerPool.ActiveWorkersCount > 0 || this.workQueue.Count > 0;

        private readonly ILinkExtractor linkExtractor;
        private readonly ICrawlRequestFilter crawlRequestFilter;
        private readonly ICrawlerObserver observer;
        private readonly IReadOnlyDictionary<string, string> customHttpHeaders;

        private readonly WorkerPool workerPool;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly BlockingCollection<CrawlRequest> workQueue = new BlockingCollection<CrawlRequest>(new ConcurrentQueue<CrawlRequest>());
        private readonly ConcurrentDictionary<Uri, CrawledUrlPropertiesImpl> discoveredUrls = new ConcurrentDictionary<Uri, CrawledUrlPropertiesImpl>();

        public Crawler(WorkerPool workerPool, ICrawlRequestFilter crawlRequestFilter, ILinkExtractor linkExtractor, ICrawlerObserver observer,
            IReadOnlyDictionary<string, string> customHttpHeaders = null, TimeSpan? requestTimeout = null)
        {
            this.crawlRequestFilter = crawlRequestFilter;
            this.linkExtractor = linkExtractor;
            this.observer = observer;
            this.customHttpHeaders = customHttpHeaders ?? new Dictionary<string, string>();
            this.workerPool = workerPool;

            if (requestTimeout != null)
            {
                this.httpClient.Timeout = requestTimeout.Value;
            }
        }

        public void Enqueue(Uri url)
        {
            this.workQueue.Add(new CrawlRequest(url));
            this.discoveredUrls.TryAdd(url, new CrawledUrlPropertiesImpl(url));
        }

        public void Enqueue(IEnumerable<Uri> urls)
        {
            foreach (var url in urls)
            {
                this.Enqueue(url);
            }
        }

        public async Task<IReadOnlyDictionary<Uri, CrawledUrlProperties>> Crawl(CancellationToken cancellationToken = default)
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
            }, cancellationToken);

            return this.discoveredUrls.ToDictionary(kvp => kvp.Key, kvp => (CrawledUrlProperties)kvp.Value);
        }

        private async Task Crawl(CrawlRequest request, CancellationToken cancellationToken)
        {
            this.observer.OnCrawling(request);

            try
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    RequestUri = request.Url,
                    Method = HttpMethod.Get,
                };


                foreach (var customHttpHeader in this.customHttpHeaders)
                {
                    httpRequestMessage.Headers.Add(customHttpHeader.Key, customHttpHeader.Value);
                }

                var requestStopWatch = Stopwatch.StartNew();
                using (var response = await this.httpClient.SendAsync(httpRequestMessage, cancellationToken))
                {
                    this.discoveredUrls[request.Url].status = response.StatusCode;



                    if (response.IsSuccessStatusCode)
                    {
                        this.observer.OnCrawled(new CrawlResult(request.Url, response.StatusCode, request.Referrer,
                            requestStopWatch.Elapsed));

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
                    else
                    {
                        this.observer.OnError(new CrawlError(request.Url, response.StatusCode, request.Referrer));
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested == false)
                {
                    // it means timeout but there is no easy way to find it out
                    // https://github.com/dotnet/corefx/issues/20296

                    throw new TaskCanceledException($"Task canceled for {request.Url}. (timeout?)", ex);
                }

                // otherwise it means request to stop processing new urls
            }
            catch (Exception e)
            {
                this.observer.OnError(new CrawlError(request.Url, e, request.Referrer));
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

            urlProperties.referrers.TryAdd(request.Url, 0);

            return newUrl;
        }

        private class CrawledUrlPropertiesImpl : CrawledUrlProperties
        {
            public override HttpStatusCode? Status => this.status;
            public override IEnumerable<Uri> Referrers => this.referrers.Keys;

            public HttpStatusCode? status;
            public readonly ConcurrentDictionary<Uri, int> referrers = new ConcurrentDictionary<Uri, int>();

            public CrawledUrlPropertiesImpl(Uri url) : base(url)
            {
            }
        }

    }
}
