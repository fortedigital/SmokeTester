using System;
using System.Linq;

namespace Forte.SmokeTester
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var crawler = new Crawler();
            var errors = crawler.Crawl(new Uri(args[0]));

            if (errors.Any())
            {
                Console.WriteLine("\nCrawl errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"{error.Status}: {error.Url} (Referer: {error.Referer})");
                }
                return 1;
            }

            return 0;
        }
    }
}
