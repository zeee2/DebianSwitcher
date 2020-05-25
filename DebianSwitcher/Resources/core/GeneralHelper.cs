using System.Net;
using System.Threading.Tasks;

namespace DebianSwitcher
{
    static class GeneralHelper
    {
        public async static Task<string> GetIPAsync()
        {
            using (var webClient = new WebClient())
            {
                string result = string.Empty;
                try
                {
                    var line = await webClient.DownloadStringTaskAsync(Constants.ip);
                    result = line;
                }
                catch { }
                return result.Trim();
            }
        }
    }
}