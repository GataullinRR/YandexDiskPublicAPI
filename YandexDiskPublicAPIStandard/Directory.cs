using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Interfaces;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    public class YandexDiskDirectoryAccessor : IDirectoryAccessor
    {
        readonly RootObject _rawData;

        public DirectoryAccess Access => DirectoryAccess.READ_ONLY;
        public Uri Path => new Uri(_rawData.path, UriKind.Relative);

        internal YandexDiskDirectoryAccessor(RootObject raw)
        {
            _rawData = raw;
        }

        //public async Task DownloadToAsync(IDirectoryAccessor directory)
        //{
        //    foreach (var file in EnumerateFilesAsync())
        //    {
        //        var d = await directory.GetFileAsync(new System.Uri(file.Name, System.UriKind.Relative)).ThenDo(async f => await f.OpenAsync(FileOpenMode.NEW);

        //        file.DownloadTo(directory);
        //    }

        //    foreach (var innerDir in Directories)
        //    {
        //        var dirPath = System.IO.Path.Combine(directory, innerDir.Name);
        //        innerDir.DownloadTo(dirPath);
        //    }
        //}

        public Task<IDirectoryAccessor> EnsureCreatedAsync()
        {
            throw new NotSupportedException();
        }

        public Task<IFileAccessor> GetFileAsync(Uri relativePath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task<IFileAccessor>> EnumerateFilesAsync()
        {
            return _rawData._embedded.items
                .Where(i => i.type == "file")
                .Select(f => Task.FromResult((IFileAccessor)new File(f, this)));
        }

        public IEnumerable<Task<IDirectoryAccessor>> EnumerateDirectoriesAsync()
        {
            foreach (var dir in _rawData._embedded.items.Where(i => i.type == "dir"))
            {
                yield return new Task<IDirectoryAccessor>(() =>
                {
                    var info = YandexDisk.PerformListRequestAsync(_rawData.public_key, dir.path).Result;
                    return new YandexDiskDirectoryAccessor(info);
                });
            }
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            throw new NotImplementedException();
        }

        public Task<IFileAccessor> GetDirectoryAsync(Uri relativePath)
        {
            throw new NotImplementedException();
        }
    }
}
