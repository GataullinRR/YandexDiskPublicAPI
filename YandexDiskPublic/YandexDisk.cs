using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    static class YandexDisk
    {
        public static Directory List(string directoryId)
        {
            var raw = PerformListRequest(directoryId);

            return new Directory(raw);
        }

        internal static RootObject PerformListRequest(string publicKey)
        {
            var request = "https://cloud-api.yandex.net/v1/disk/public/resources?public_key=" + HttpUtility.HtmlEncode(publicKey);
            var answer = Utils.DoGetRequest(request);
            var graph = JsonConvert.DeserializeObject<RootObject>(answer);

            return JsonConvert.DeserializeObject<RootObject>(answer);
        }

        internal static RootObject PerformListRequest(string publicKey, string path)
        {
            var requestTemplate = "https://cloud-api.yandex.net/v1/disk/public/resources?public_key={0}&path={1}";
            var request = string.Format(requestTemplate, HttpUtility.HtmlEncode(publicKey), HttpUtility.UrlPathEncode(path));
            var answer = Utils.DoGetRequest(request);

            return JsonConvert.DeserializeObject<RootObject>(answer);
        }

        internal static DownloadAnswer PerformDownloadRequest(string publicKey, string path)
        {
            var requestTemplate = "https://cloud-api.yandex.net/v1/disk/public/resources/download?public_key={0}&path={1}";
            var request = string.Format(requestTemplate, HttpUtility.HtmlEncode(publicKey), HttpUtility.UrlPathEncode(path));
            var answer = Utils.DoGetRequest(request);

            return JsonConvert.DeserializeObject<DownloadAnswer>(answer);
        }
    }
}
