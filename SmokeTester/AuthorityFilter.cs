using System;

namespace Forte.SmokeTester
{
    public class AuthorityFilter : ICrawlRequestFilter
    {
        private readonly string authority;

        public AuthorityFilter(string authority)
        {
            this.authority = authority;
        }

        public bool ShouldCrawl(CrawlRequest request)
        {
            return this.authority.Equals(request.Url.Authority, StringComparison.OrdinalIgnoreCase);
        }
    }
}