using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    public static class YandexDisk
    {
        public static async Task<YandexDiskDirectoryAccessor> ListAsync(string directoryId)
        {
            var raw = await PerformListRequestAsync(directoryId);

            return new YandexDiskDirectoryAccessor(raw);
        }

        internal static async Task<RootObject> PerformListRequestAsync(string publicKey)
        {
            var request = "https://cloud-api.yandex.net/v1/disk/public/resources?public_key=" + HttpUtility.HtmlEncode(publicKey);
            var answer = await Utils.DoGetRequestAsync(request);

            return JsonConvert.DeserializeObject<RootObject>(answer);
        }

        internal static async Task<RootObject> PerformListRequestAsync(string publicKey, string path)
        {
            var requestTemplate = "https://cloud-api.yandex.net/v1/disk/public/resources?public_key={0}&path={1}";
            var request = string.Format(requestTemplate, HttpUtility.HtmlEncode(publicKey), HttpUtility.UrlPathEncode(path));
            var answer = await Utils.DoGetRequestAsync(request);

            return JsonConvert.DeserializeObject<RootObject>(answer);
        }

        internal static async Task<DownloadAnswer> PerformDownloadRequestAsync(string publicKey, string path)
        {
            var requestTemplate = "https://cloud-api.yandex.net/v1/disk/public/resources/download?public_key={0}&path={1}";
            var request = string.Format(requestTemplate, HttpUtility.HtmlEncode(publicKey), HttpUtility.UrlPathEncode(path));
            var answer = await Utils.DoGetRequestAsync(request);

            return JsonConvert.DeserializeObject<DownloadAnswer>(answer);
        }
    }
}
