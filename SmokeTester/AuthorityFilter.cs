using System;
using System.Collections.Generic;
using System.Linq;

namespace Forte.SmokeTester
{
    public class AuthorityFilter : ICrawlRequestFilter
    {
        private readonly bool testExternalUrls;
        private readonly IReadOnlyCollection<string> authorities;

        public AuthorityFilter(IEnumerable<string> authorities, bool testExternalUrls)
        {
            this.testExternalUrls = testExternalUrls;
            this.authorities = authorities.Distinct().ToList();

            if (this.authorities.Count == 0)
            {
                throw new ArgumentException("Expected at least one authority but found 0", nameof(authorities));
            }
        }

        public bool ShouldCrawl(CrawlRequest request)
        {
            var requestUrl = request.Url;
            if (this.MatchesAuthority(requestUrl))
            {
                return true;
            }

            // test only 1st level external urls
            return this.testExternalUrls && this.MatchesAuthority(request.Referrer);
        }

        private bool MatchesAuthority(Uri requestUrl)
        {
            return this.authorities.Any(x => x.Equals(requestUrl.Authority, StringComparison.OrdinalIgnoreCase));
        }
    }
}