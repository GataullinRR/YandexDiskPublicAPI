using System;
using System.IO;
using System.Threading.Tasks;
using Utilities.Interfaces;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    public class File : IFileAccessor
    {
        readonly Item _rawData;
        readonly IDirectoryAccessor _rootDirectory;

        public string Name => _rawData.name;
        public int Size => _rawData.size;

        public Utilities.Interfaces.FileAccess Access => Utilities.Interfaces.FileAccess.READ_ONLY;

        internal File(Item fileInfo, IDirectoryAccessor rootDirectory)
        {
            _rawData = fileInfo;
            _rootDirectory = rootDirectory;
        }

        public async Task<Stream> OpenAsync(FileOpenMode mode)
        {
            if (mode != FileOpenMode.OPEN)
            {
                throw new NotSupportedException();
            }

            var raw = await YandexDisk.PerformDownloadRequestAsync(_rawData.public_key, _rawData.path);
            var file = await Utils.DownloadAsync(raw.href);
            return new MemoryStream(file);
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            return _rootDirectory;
        }
    }
}
