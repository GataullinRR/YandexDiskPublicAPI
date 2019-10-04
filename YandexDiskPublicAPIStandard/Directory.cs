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
    public class YandexDiskDirectory : IDirectoryAccessor
    {
        readonly RootObject _rawData;
        readonly string _publicUrl;

        public DirectoryAccess Access => DirectoryAccess.READ_ONLY;
        public string Name => _rawData.name;

        internal YandexDiskDirectory(RootObject raw, string publicUrl)
        {
            _rawData = raw;
            _publicUrl = publicUrl;
        }

        public async Task DownloadToAsync(IDirectoryAccessor directory, System.Threading.CancellationToken cancellation)
        {
            foreach (var fileTask in EnumerateFilesAsync())
            {
                var file = await fileTask;
                var destinationFile = await directory.GetFileAsync(new Uri(file.Name, UriKind.Relative), cancellation);
                using (var destinationFileStream = await destinationFile.OpenAsync(FileOpenMode.NEW, cancellation))
                using (var fileStream = await file.OpenAsync(FileOpenMode.OPEN, cancellation))
                {
                    await fileStream.WriteToAsync(destinationFileStream, cancellation);
                }
            }

            foreach (var innerDir in EnumerateDirectoriesAsync(cancellation))
            {
                var dir = await innerDir;
                var destinationDirectoryPath = new Uri(dir._rawData.path.Remove(_rawData.path), UriKind.Relative);
                var destinationDirectory = await directory.GetDirectoryAsync(destinationDirectoryPath, cancellation);
                await dir.DownloadToAsync(destinationDirectory, cancellation);
            }
        }

        public Task<IDirectoryAccessor> EnsureCreatedAsync(System.Threading.CancellationToken cancellation)
        {
            throw new NotSupportedException("The directory is read only");
        }

        Task<IFileAccessor> IDirectoryAccessor.GetFileAsync(Uri relativePath, System.Threading.CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
        public Task<YandexDiskFile> GetFileAsync(Uri relativePath)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Task<IFileAccessor>> IDirectoryAccessor.EnumerateFilesAsync(System.Threading.CancellationToken cancellation)
        {
            foreach (var file in EnumerateFilesAsync())
            {
                Func<Task<IFileAccessor>> getFile = async () => await file;

                yield return getFile();
            }
        }
        public IEnumerable<Task<YandexDiskFile>> EnumerateFilesAsync()
        {
            return _rawData._embedded.items
                .Where(i => i.type == "file")
                .Select(f => Task.FromResult(new YandexDiskFile(f, this)));
        }

        IEnumerable<Task<IDirectoryAccessor>> IDirectoryAccessor.EnumerateDirectoriesAsync(System.Threading.CancellationToken cancellation)
        {
            foreach (var dir in EnumerateDirectoriesAsync(cancellation))
            {
                Func<Task<IDirectoryAccessor>> getDir = async () => await dir;

                yield return getDir();
            }
        }
        public IEnumerable<Task<YandexDiskDirectory>> EnumerateDirectoriesAsync(System.Threading.CancellationToken cancellation)
        {
            foreach (var dir in _rawData._embedded.items.Where(i => i.type == "dir"))
            {
                var localPath = dir.path.Remove(0, _rawData.path.Length);

                yield return GetDirectoryAsync(new Uri(localPath, UriKind.Relative), cancellation);
            }
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            throw new NotImplementedException();
        }

        async Task<IDirectoryAccessor> IDirectoryAccessor.GetDirectoryAsync(Uri relativePath, System.Threading.CancellationToken cancellation)
        {
            return await GetDirectoryAsync(relativePath, cancellation);
        }
        public async Task<YandexDiskDirectory> GetDirectoryAsync(Uri relativePath, System.Threading.CancellationToken cancellation)
        {
            var path = _rawData.path + relativePath.ToString();
            if (path.Contains("//"))
            {
                throw new ArgumentOutOfRangeException("Path is not valid");
            }
            var answer = await YandexDisk.PerformListRequestAsync(_publicUrl, path, cancellation);

            return new YandexDiskDirectory(answer, _publicUrl);
        }
    }
}
