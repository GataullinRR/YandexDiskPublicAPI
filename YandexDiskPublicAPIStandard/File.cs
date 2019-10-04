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

        public string Name => _rawData.name;
        public int Size => _rawData.size;

        public Utilities.Interfaces.FileAccess Access => Utilities.Interfaces.FileAccess.READ_ONLY;

        internal YandexDiskFile(Item fileInfo, IDirectoryAccessor rootDirectory)
        {
            _rawData = fileInfo;
            _rootDirectory = rootDirectory;
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            return _rootDirectory;
        }

        public async Task<Stream> OpenAsync(FileOpenMode mode, CancellationToken cancellation)
        {
            if (mode != FileOpenMode.OPEN)
            {
                throw new NotSupportedException();
            }

            var raw = await YandexDisk.PerformDownloadRequestAsync(_rawData.public_key, _rawData.path, cancellation);
            var file = await Utils.DownloadAsync(raw.href, cancellation);
            return new MemoryStream(file);
        }
    }
}
