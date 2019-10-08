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
        static readonly PathFormat PATH_FORMAT = new PathFormat("/");

        readonly RootObject _rawData;
        readonly string _publicUrl;
        readonly UPath _path;

        public IOAccess Access => IOAccess.READ_ONLY;
        public string Name => _rawData.name;


        internal YandexDiskDirectory(RootObject raw, string publicUrl)
        {
            _rawData = raw;
            _publicUrl = publicUrl;
            _path = new UPath(PATH_FORMAT, _rawData.path);
        }

        public async Task DownloadToAsync(IDirectoryAccessor destinationRootDirectory, System.Threading.CancellationToken cancellation)
        {
            if (destinationRootDirectory.Access != IOAccess.FULL)
            {
                throw new NotSupportedException("Access denied");
            }

            foreach (var fileTask in EnumerateFilesAsync())
            {
                var file = await fileTask;
                var destinationFile = await destinationRootDirectory.GetFileAsync(new UPath(PATH_FORMAT, file.Name), cancellation);
                using (var destinationFileStream = await destinationFile.OpenAsync(FileOpenMode.NEW, cancellation))
                using (var fileStream = await file.OpenAsync(FileOpenMode.OPEN_OR_NEW, cancellation))
                {
                    await fileStream.WriteToAsync(destinationFileStream, cancellation);
                }
            }

            foreach (var innerDir in EnumerateDirectoriesAsync(cancellation))
            {
                var dir = await innerDir;
                var destinationDirectoryPath = new UPath(PATH_FORMAT, dir._rawData.path.RemoveFirst(_rawData.path));
                var destinationDirectory = await destinationRootDirectory.GetDirectoryAsync(destinationDirectoryPath, cancellation);
                await destinationDirectory.EnsureCreatedAsync(cancellation);
                await dir.DownloadToAsync(destinationDirectory, cancellation);
            }
        }

        public Task<IDirectoryAccessor> EnsureCreatedAsync(System.Threading.CancellationToken cancellation)
        {
            throw new NotSupportedException("The directory is read only");
        }

        Task<IFileAccessor> IDirectoryAccessor.GetFileAsync(UPath relativePath, System.Threading.CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
        public Task<YandexDiskFile> GetFileAsync(UPath relativePath)
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
                .Select(f => Task.FromResult(new YandexDiskFile(f, this, _publicUrl)));
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

                yield return GetDirectoryAsync(new UPath(PATH_FORMAT, localPath), cancellation);
            }
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            throw new NotImplementedException();
        }

        async Task<IDirectoryAccessor> IDirectoryAccessor.GetDirectoryAsync(UPath relativePath, System.Threading.CancellationToken cancellation)
        {
            return await GetDirectoryAsync(relativePath, cancellation);
        }
        public async Task<YandexDiskDirectory> GetDirectoryAsync(UPath relativePath, System.Threading.CancellationToken cancellation)
        {
            var path = _path.GetCopy().AppendSelf(relativePath).ToString();
            var answer = await YandexDisk.PerformListRequestAsync(_publicUrl, path, cancellation);

            return new YandexDiskDirectory(answer, _publicUrl);
        }
    }
}
