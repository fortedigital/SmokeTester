# SmokeTester
Simple command line web spider that crawls a provided url and reports any HTTP errors (404, 500, etc). 
Crawling is limited to the host of the provided start url.
Can be used to smoke-test a website.

Usage:

  -u, --url        Required. Start url where the crawling will begin.

  -d, --depth      (Default: 3) Maximum deapth of url to extract.

  --maxErrors      (Default: 100) Number of errors after which the crawler is stoped.

  -w, --workers    (Default: 3) Number of workers.

  --maxUrls        (Default: 1000) Number of urls after which the crawler is stoped.
