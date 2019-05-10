﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public interface ILinkExtractor
    {
        Task<IReadOnlyCollection<Uri>> ExtractLinks(CrawlRequest crawlRequest, HttpContent content);
    }
}