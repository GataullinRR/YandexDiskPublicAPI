using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using YandexDiskPublicAPI;
using Utilities.Extensions;
using System.Security.Policy;
using Utilities;

namespace APITests
{
    public class UnitTest1
    {
        public async Task<YandexDiskDirectory> getRoot() => await YandexDisk.GetRootAsync("https://yadi.sk/d/RVA1vVoQBq_LOw", Utilities.ThreadingUtils.CreateCToken(10000));

        [Fact]
        public async Task FolderNavigationTest()
        {
            var cancellation = ThreadingUtils.CreateCToken(10000);
            var rootDirectory = await getRoot();
            var folder1 = await rootDirectory.GetDirectoryAsync(new Uri("Folder 1", UriKind.Relative), cancellation);
            var folder1_1 = await rootDirectory.GetDirectoryAsync(new Uri("Folder 1/Folder 1-1", UriKind.Relative), cancellation);
            var folder1_2 = await rootDirectory.GetDirectoryAsync(new Uri("Folder 1/Folder 1-2", UriKind.Relative), cancellation);

            await validateContent(rootDirectory, "Folder 1", "File in the root folder.txt", cancellation);
            await validateContent(folder1, "Folder 1-1,Folder 1-2", "File in the folder 1.txt,Second file in the folder 1.txt", cancellation);
            await validateContent(folder1_1, null, null, cancellation);
            await validateContent(folder1_2, null, "File in the folder 1-2.txt", cancellation);
        }

        async Task validateContent(YandexDiskDirectory directory, string folderNames, string fileNames, System.Threading.CancellationToken cancellation)
        {
            var directories = directory.EnumerateDirectoriesAsync(cancellation).Select(d => d.Result.CastTo<YandexDiskDirectory>().Name).ToArray();
            var files = directory.EnumerateFilesAsync().Select(f => f.Result.CastTo<YandexDiskFile>().Name).ToArray();

            var expectedFolders = folderNames?.Split(",") ?? new string[0];
            var expectedFiles = fileNames?.Split(",") ?? new string[0];

            Assert.Equal(expectedFolders, directories);
            Assert.Equal(expectedFiles, files);
        }
    }
}
