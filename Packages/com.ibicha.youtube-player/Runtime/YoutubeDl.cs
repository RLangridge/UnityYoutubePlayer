using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace YoutubePlayer
{
    public class YoutubeDl
    {
        public static string ServerUrl { get; set; } = "https://unity-youtube-dl-server.herokuapp.com";

        public static async Task<T> GetVideoMetaDataAsync<T>(string youtubeUrl, CancellationToken cancellationToken = default)
        {
            return await GetVideoMetaDataAsync<T>(youtubeUrl, YoutubeDlOptions.Default, cancellationToken);
        }

        public static async Task<T> GetVideoMetaDataAsync<T>(string youtubeUrl, YoutubeDlOptions options,
            CancellationToken cancellationToken = default)
        {
            var optionFlags = new List<string>();
            if (!string.IsNullOrWhiteSpace(options.Format))
            {
                optionFlags.Add($"-f \"{options.Format}\"");
            }

            if (options.UserAgent != null)
            {
                //optionFlags.Add($"--user-agent \"{options.UserAgent}\"");
            }

            if (options.Custom != null)
            {
                optionFlags.Add(options.Custom);
            }

            var requestUrl = $"http://{ServerUrl}/v1/video?url={youtubeUrl}";
            if (optionFlags.Count > 0)
            {
                //requestUrl += $"&options={UnityWebRequest.EscapeURL(string.Join(" ", optionFlags))}";
            }
            
            using (var client = new HttpClient())
            {
                try
                {
                    var request = await client.GetAsync(requestUrl, cancellationToken);
                    var tcs = new TaskCompletionSource<T>();
                    var text = await request.Content.ReadAsStringAsync();
                    var video = JsonConvert.DeserializeObject<T>(text);
                    tcs.TrySetResult(video);
                    return await tcs.Task;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Youtube Error: {e}");
                    return default;
                }
            }
        }
        
    }
}