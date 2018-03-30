# SmokeTester
Simple command line web spider that crawls a provided url and reports any HTTP errors (404, 500, etc). 
Crawling is limited to the host of the provided start url.
Can be used to smoke-test a website.

Usage:

### Crawl mode
You pass start url and SmokeTester finds links by itself.

#### Usage:

`Forte.SmokeTester.exe crawl -u http://example.com`

Possible options: 

  -u, --url        Required. Start url where the crawling will begin.

  -d, --depth      (Default: 3) Maximum deapth of url to extract.

  --maxErrors      (Default: 100) Number of errors after which the crawler is stoped.

  -w, --workers    (Default: 3) Number of workers.

  --maxUrls        (Default: 1000) Number of urls after which the crawler is stoped.


### Sitemap mode

You pass sitemap.xml url and SmokeTester checks all links included in Sitemap

#### Usage

`Forte.SmokeTester.exe sitemap -s http://example.com/sitemap.xml`

Possible options:

  -s, --sitemapurl    Required. Url of sitemap to check

  --maxErrors         (Default: 100) Number of errors after which the crawler is stoped.

  -w, --workers       (Default: 3) Number of workers.

  --maxUrls           (Default: 1000) Number of urls after which the crawler is stoped.

  --help              Display this help screen.

  --version           Display version information.
