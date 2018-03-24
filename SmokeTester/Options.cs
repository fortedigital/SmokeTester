using CommandLine;

namespace Forte.SmokeTester
{
    internal class Options
    {
        [Option('u', "url", Required = true, HelpText = "Start url where the crawling will begin.")]
        public string StartUrl { get; set; }
        
        [Option('d', "depth", Default = 3, HelpText = "Maximum deapth of url to extract.")]
        public int MaxDepth { get; set; }
        
        [Option("maxErrors", Default = 100, HelpText = "Number of errors after which the crawler is stoped.")]
        public int MaxErrors { get; set; }

        [Option('w', "workers", Default = 3, HelpText = "Number of workers.")]
        public int NumberOfWorkers { get; set; }
        
        [Option("maxUrls", Default = 1000, HelpText = "Number of urls after which the crawler is stoped.")]
        public int MaxUrls { get; set; }
    }
}