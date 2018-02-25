using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MockHttpClient
{
    /// <summary>
    /// Provides extension methods on <see cref="HttpContent"/>
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Reads content as deserialized json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content">The http content.</param>
        /// <param name="settings">The settings to use when deserializing.</param>
        /// <returns></returns>
        public static async Task<T> ReadAsJson<T>(this HttpContent content, JsonSerializerSettings settings = null)
        {
            if (content == null)
                return default(T);

            var serializer = JsonSerializer.CreateDefault(settings);
            using (var sr = new StreamReader(await content.ReadAsStreamAsync()))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        /// <summary>
        /// Clones the http content.
        /// </summary>
        /// <param name="content">The http content to cline.</param>
        /// <returns>A new HttpContent with the same content as the original</returns>
        public static HttpContent Clone(this HttpContent content)
        {
            var ms = new MemoryStream();
            content.CopyToAsync(ms).Wait();
            ms.Position = 0;

            var newContent = new StreamContent(ms);

            foreach (var v in content.Headers)
                newContent.Headers.TryAddWithoutValidation(v.Key, v.Value);

            return newContent;
        }
    }
}
