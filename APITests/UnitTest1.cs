using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using YandexDiskPublicAPI;
using Utilities.Extensions;
using System.Security.Policy;
using Utilities;
using Utilities.Interfaces;
using System.IO;

namespace APITests
{
    public class UnitTest1
    {
        public async Task<YandexDiskDirectory> getRoot() => await YandexDisk.GetRootAsync("https://yadi.sk/d/RVA1vVoQBq_LOw", Utilities.ThreadingUtils.CreateCToken(10000));

        static readonly PathFormat PATH_FORMAT = new PathFormat("\\", "\\:");

        [Fact]
        public async Task FolderDownloadTest()
        {
            var cancellation = ThreadingUtils.CreateCToken(1000000000);
            var targetDirectoryPath = Path.Combine(Path.GetTempPath(), "Test");
            IOUtils.RecreateDirectory(targetDirectoryPath);
            var targetDirectory = new DiskDirectory(targetDirectoryPath, IOAccess.FULL);
            await (await getRoot()).DownloadToAsync(targetDirectory, cancellation);

            var folder1 = await targetDirectory.GetDirectoryAsync(new UPath(PATH_FORMAT, "Folder 1"), cancellation);
            var folder1_1 = await targetDirectory.GetDirectoryAsync(new UPath(PATH_FORMAT, "Folder 1\\Folder 1-1"), cancellation);
            var folder1_2 = await targetDirectory.GetDirectoryAsync(new UPath(PATH_FORMAT, "Folder 1\\Folder 1-2"), cancellation);

            await validateContent(targetDirectory, "Folder 1", "File in the root folder.txt", cancellation);
            await validateContent(folder1, "Folder 1-1,Folder 1-2", "File in the folder 1.txt,Second file in the folder 1.txt", cancellation);
            await validateContent(folder1_1, null, null, cancellation);
            await validateContent(folder1_2, null, "File in the folder 1-2.txt", cancellation);
        }

        [Fact]
        public async Task FolderNavigationTest()
        {
            var cancellation = ThreadingUtils.CreateCToken(10000);
            var rootDirectory = await getRoot();
            var folder1 = await rootDirectory.GetDirectoryAsync(new UPath(PATH_FORMAT, "Folder 1"), cancellation);
            var folder1_1 = await rootDirectory.GetDirectoryAsync(new UPath(PATH_FORMAT, "Folder 1\\Folder 1-1"), cancellation);
            var folder1_2 = await rootDirectory.GetDirectoryAsync(new UPath(PATH_FORMAT, "Folder 1\\Folder 1-2"), cancellation);

            await validateContent(rootDirectory, "Folder 1", "File in the root folder.txt", cancellation);
            await validateContent(folder1, "Folder 1-1,Folder 1-2", "File in the folder 1.txt,Second file in the folder 1.txt", cancellation);
            await validateContent(folder1_1, null, null, cancellation);
            await validateContent(folder1_2, null, "File in the folder 1-2.txt", cancellation);
        }

        async Task validateContent(IDirectoryAccessor directory, string folderNames, string fileNames, System.Threading.CancellationToken cancellation)
        {
            var directories = directory.EnumerateDirectoriesAsync(cancellation).Select(d => d.Result.Name).ToArray();
            var files = directory.EnumerateFilesAsync(cancellation).Select(f => f.Result.Name).ToArray();

            var expectedFolders = folderNames?.Split(",") ?? new string[0];
            var expectedFiles = fileNames?.Split(",") ?? new string[0];

            Assert.Equal(expectedFolders, directories);
            Assert.Equal(expectedFiles, files);
        }
    }
}
