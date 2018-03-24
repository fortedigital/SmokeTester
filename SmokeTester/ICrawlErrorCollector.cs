using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public interface ICrawlErrorCollector
    {
        Task CollectError(CrawlError error);
    }
}