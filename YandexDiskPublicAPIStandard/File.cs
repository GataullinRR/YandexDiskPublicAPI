using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Interfaces;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    public class YandexDiskFile : IFileAccessor
    {
        readonly Item _rawData;
        readonly IDirectoryAccessor _rootDirectory;
        readonly string _publicUrl;

        public string Name => _rawData.name;
        public int Size => _rawData.size;

        public IOAccess Access => IOAccess.READ_ONLY;

        internal YandexDiskFile(Item fileInfo, IDirectoryAccessor rootDirectory, string publicUrl)
        {
            _rawData = fileInfo;
            _rootDirectory = rootDirectory;
            _publicUrl = publicUrl;
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            return _rootDirectory;
        }

        public async Task<Stream> OpenAsync(FileOpenMode mode, CancellationToken cancellation)
        {
            if (mode != FileOpenMode.OPEN_OR_NEW)
            {
                throw new NotSupportedException();
            }

            var raw = await YandexDisk.PerformDownloadRequestAsync(_publicUrl, _rawData.path, cancellation);
            var file = await Utils.DownloadAsync(raw.href, cancellation);
            return new MemoryStream(file);
        }
    }
}
