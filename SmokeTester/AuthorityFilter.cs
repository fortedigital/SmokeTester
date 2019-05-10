using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Forte.SmokeTester
{
    public class AuthorityFilter : ICrawlRequestFilter
    {
        private readonly IReadOnlyCollection<string> authorities;

        public AuthorityFilter(IEnumerable<string> authorities)
        {
            this.authorities = authorities.Distinct().ToList();

            if (this.authorities.Count == 0)
            {
                throw new ArgumentException("Expected at least authority but found 0",nameof(authorities));
            }
        }

        public bool ShouldCrawl(CrawlRequest request)
        {
            return this.authorities.Any(x=>x.Equals(request.Url.Authority, StringComparison.OrdinalIgnoreCase));
        }
    }
}