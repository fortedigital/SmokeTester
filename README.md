# SmokeTester
Simple command line web spider that crawls a provided url and reports any HTTP errors (404, 500, etc). 
Crawling is limited to the host of the provided start url.
Can be used to smoke-test a website.

Usage:

  -u, --url        Required. Start url where the crawling will begin. You can pass sitemap url as well.

  -d, --depth      (Default: 3) Maximum deapth of url to extract.

  --maxErrors      (Default: 100) Number of errors after which the crawler is stoped.

  -w, --workers    (Default: 3) Number of workers.

  --maxUrls        (Default: 1000) Number of urls after which the crawler is stoped.

  --no-robots      Set this flag to disable parsing robots.txt in order to search for sitemaps
  

This project makes use of [RobotsTxt](https://bitbucket.org/cagdas/robotstxt) package, slightly modified to meet `.NetStandard` requirements. Once [this PR](https://bitbucket.org/cagdas/robotstxt/pull-requests/1/adjust-library-to-be-used-with-net-core/diff#comment-None) is merged `SmokeTester` will switch to [nuget package](https://www.nuget.org/packages/RobotsTxt/) 
