using CommandLine;

namespace Forte.SmokeTester
{
    internal abstract class CommonOptions
    {
        [Option("maxErrors", Default = 100, HelpText = "Number of errors after which the crawler is stoped.")]
        public int MaxErrors { get; set; }

        [Option('w', "workers", Default = 3, HelpText = "Number of workers.")]
        public int NumberOfWorkers { get; set; }
        
        [Option("maxUrls", Default = 1000, HelpText = "Number of urls after which the crawler is stoped.")]
        public int MaxUrls { get; set; }

        public abstract string GetStartUrl();
    }

    [Verb("crawl", HelpText = "Crawl mode. Provide entry url as a starting point")]
    internal class CrawlOptions : CommonOptions
    {
        [Option('d', "depth", Default = 3, HelpText = "Maximum deapth of url to extract.")]
        public int MaxDepth { get; set; }
        
        [Option('s', "starturl", Required = true, HelpText = "Start url where the crawling will begin.")]
        public string StartUrl { get; set; }

        public override string GetStartUrl()
        {
            return StartUrl;
        }
    }
    
    [Verb("sitemap", HelpText = "Sitemap mode. Provide url to sitemap to check")]
    internal class SitemapOptions : CommonOptions
    {
        [Option('s', "sitemapurl", Required = true, HelpText = "Url of sitemap to check")]
        public string SitemapUrl { get; set; }

        public override string GetStartUrl()
        {
            return SitemapUrl;
        }
    }
}