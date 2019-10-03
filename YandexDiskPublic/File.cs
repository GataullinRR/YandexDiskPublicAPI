using System.IO;
using Utilities;
using Utilities.Extensions;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    public class File
    {
        protected readonly Item _rawData;

        public string Name => _rawData.name;
        public int Size => _rawData.size;

        public File(Item fileInfo)
        {
            _rawData = fileInfo;
        }

        public void DownloadTo(string directory)
        {
            var path = Path.Combine(directory, Name);
            using (var stream = IOUtils.CreateFile(path))
            {
                DownloadTo(stream);
            }
        }
        public void DownloadTo(Stream stream)
        {
            var raw = YandexDisk.PerformDownloadRequest(_rawData.public_key, _rawData.path);
            var file = Utils.Download(raw.href);
            stream.Write(file);
        }
    }
}
