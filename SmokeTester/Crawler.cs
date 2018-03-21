using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Html;
using AngleSharp.Parser.Html;

namespace Forte.SmokeTester
{
    public class Crawler
    {
        private readonly BlockingCollection<Worker> workersPool = new BlockingCollection<Worker>(new ConcurrentBag<Worker>());
        private readonly BlockingCollection<CrawlRequest> workQueue = new BlockingCollection<CrawlRequest>(new ConcurrentQueue<CrawlRequest>());
        private readonly ConcurrentDictionary<Uri, Uri> visitedUrls = new ConcurrentDictionary<Uri, Uri>();
        
        private readonly int maxWorkers;
        private readonly HttpClient httpClient = new HttpClient();

        public Crawler(int maxWorkers = 3)
        {
            this.maxWorkers = maxWorkers;
            for (var i = 0; i < maxWorkers; i++)
            {
                this.workersPool.Add(new Worker(this));
            }
        }

        public IEnumerable<CrawlError> Crawl(Uri startUrl)
        {
            var errors = new ConcurrentBag<CrawlError>();
            
            this.workQueue.Add(new CrawlRequest(startUrl));
            this.visitedUrls.TryAdd(startUrl, startUrl);
            
            while (this.workQueue.Count > 0 || this.maxWorkers - this.workersPool.Count > 0)
            {
                if (this.workQueue.TryTake(out var request, TimeSpan.FromSeconds(5)) == false)
                    continue;
                if (this.workersPool.TryTake(out var worker, TimeSpan.FromSeconds(5)) == false)
                    continue;
                
                worker.Run(async () =>
                {
                    Console.WriteLine(request.Url.ToString());

                    try
                    {
                        using (var response = await this.httpClient.GetAsync(request.Url))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var document = await ParseDocumentFromResponse(response);

                                foreach (var link in document.Links)
                                {
                                    var u = new Uri(request.Url, new Uri(link.GetAttribute(AttributeNames.Href), UriKind.RelativeOrAbsolute));
                                    if (this.visitedUrls.TryAdd(u, u) == false)
                                        continue;

                                    if (u.Host != startUrl.Host)
                                        continue;

                                    this.workQueue.Add(new CrawlRequest(u, request.Url));
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{response.StatusCode}: {request.Url}");
                                errors.Add(new CrawlError(request.Url, response.StatusCode, request.Referer));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception: {request.Url}\n{e}");
                        errors.Add(new CrawlError(request.Url, HttpStatusCode.Unused, request.Referer));
                    }                    
                });
            }

            return errors;
        }

        private static async Task<IHtmlDocument> ParseDocumentFromResponse(HttpResponseMessage response)
        {
            IHtmlDocument document;
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                var parser = new HtmlParser();
                document = await parser.ParseAsync(contentStream);
            }

            return document;
        }

        private class CrawlRequest
        {
            public readonly Uri Url;
            public readonly Uri Referer;

            public CrawlRequest(Uri url, Uri referer = null)
            {
                Url = url;
                Referer = referer;
            }
        }

        private class Worker
        {
            private readonly Crawler crawler;

            public Worker(Crawler crawler)
            {
                this.crawler = crawler;
            }

            public async void Run(Func<Task> task)
            {
                try
                {
                    await task();
                }
                finally
                {
                    this.crawler.workersPool.Add(this);                    
                }
            }
        }
    }
}