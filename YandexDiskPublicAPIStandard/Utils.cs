﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskPublicAPI
{
    static class Utils
    {
#warning Add cancellation
        public static async Task<string> DoGetRequestAsync(string uri, System.Threading.CancellationToken cancellation)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task<byte[]> DownloadAsync(string url, System.Threading.CancellationToken cancellation)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(url, cancellation))
                {
                    if (result.IsSuccessStatusCode)
                    {
#warning Add cancellation
                        return await result.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
