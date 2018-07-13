using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskPublicAPI;
using Utilities.Extensions;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dddd = YandexDisk.List("https://yadi.sk/d/rhaUJM4J3Z6qwQ");

            var dir = Path.Combine(Environment.CurrentDirectory, "Downloads");
            //dddd.Files.ForEach(f => f.DownloadTo(dir));
            dddd.DownloadTo(dir);

        }
    }
}
