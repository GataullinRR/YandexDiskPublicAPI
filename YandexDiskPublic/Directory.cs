using System.Collections.Generic;
using System.Linq;
using Utilities;
using YandexDiskPublicAPI.JSONObjects;

namespace YandexDiskPublicAPI
{
    public class Directory
    {
        protected readonly RootObject _rawData;

        public string Name => _rawData.name;
        public string Path => _rawData.path;
        public IEnumerable<File> Files { get; }
        public IEnumerable<Directory> Directories { get; }

        public Directory(RootObject raw)
        {
            _rawData = raw;

            Files = _rawData._embedded.items
                .Where(i => i.type == "file")
                .Select(f => new File(f));
            Directories = enumerateDirs();

            IEnumerable<Directory> enumerateDirs()
            {
                foreach (var dir in raw._embedded.items.Where(i => i.type == "dir"))
                {
                    var info = YandexDisk.PerformListRequest(raw.public_key, dir.path);
                    yield return new Directory(info);
                }
            }
        }

        public void DownloadTo(string directory)
        {
            IOUtils.CreateDirectoryIfNotExist(directory);
            foreach (var file in Files)
            {
                file.DownloadTo(directory);
            }
            foreach (var innerDir in Directories)
            {
                var dirPath = System.IO.Path.Combine(directory, innerDir.Name);
                innerDir.DownloadTo(dirPath);
            }
        }
    }
}
