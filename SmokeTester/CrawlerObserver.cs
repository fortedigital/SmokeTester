using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Forte.SmokeTester
{
    internal class CrawlerObserver : ICrawlerObserver
    {
        private readonly ConcurrentBag<Uri> crawledUrls = new ConcurrentBag<Uri>();
        public IReadOnlyCollection<Uri> CrawledUrls => this.crawledUrls;

        private readonly ConcurrentBag<CrawlError> warnings = new ConcurrentBag<CrawlError>();
        public IReadOnlyCollection<CrawlError> Warnings => this.warnings;

        private readonly ConcurrentBag<CrawlError> errors = new ConcurrentBag<CrawlError>();
        public IReadOnlyCollection<CrawlError> Errors => this.errors;
        
        private readonly int? maxErrors;
        private readonly int? maxUrls;
        private readonly CancellationTokenSource cancelationTokenSource;

        public CrawlerObserver(CancellationTokenSource cancelationTokenSource, int? maxErrors, int? maxUrls)
        {
            this.cancelationTokenSource = cancelationTokenSource;
            this.maxErrors = maxErrors;
            this.maxUrls = maxUrls;
        }

        public void OnError(CrawlError error)
        {
            if (error.Exception != null)
            {
                Console.WriteLine($"ERROR: {error.Exception.Message}: {error.Url}");
                
                this.errors.Add(error);
            }
            else
            {
                Console.WriteLine($"{error.Status}: {error.Url}" + (error.Referer == null ? string.Empty : $"(Referer: {error.Referer})"));
                
                if (error.Status == HttpStatusCode.NotFound)
                    this.warnings.Add(error);
                else
                    this.errors.Add(error);
            }

            if (this.maxErrors.HasValue && this.errors.Count >= this.maxErrors)
                this.cancelationTokenSource.Cancel();
        }

        public void OnCrawling(CrawlRequest request)
        {
            Console.WriteLine(request.Url);

            this.crawledUrls.Add(request.Url);
            
            if (this.maxUrls.HasValue && this.crawledUrls.Count >= this.maxUrls)
                this.cancelationTokenSource.Cancel();
        }

        public void OnNewUrl(Uri url)
        {
        }
    }
}