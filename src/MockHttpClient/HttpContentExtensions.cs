using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
        /// Determines whether the content is the specified string.
        /// </summary>
        /// <param name="content">The http content.</param>
        /// <param name="stringValue">The string the content is compared against.</param>
        /// <returns>
        ///   <c>true</c> if the request contains the specified string; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   stringValue
        /// </exception>
        public static bool IsString(this HttpContent content, string stringValue)
        {
            if (stringValue == null) throw new ArgumentNullException(nameof(stringValue));
            if (content == null)
                return false;

            return content.ReadAsStringAsync().GetAwaiter().GetResult() == stringValue;
        }

        /// <summary>
        /// Determines whether the content contains the specified json object.
        /// <paramref name="objectValue"/> is compared by using <c>Equals</c> with the deserialized content of the request.
        /// </summary>
        /// <typeparam name="T">The type of the object the content is compared against.</typeparam>
        /// <param name="content">The http content.</param>
        /// <param name="objectValue">The object the content is compared against.</param>
        /// <param name="jsonSerializerSettings">The settings to use when deserializing.</param>
        /// <returns>
        ///   <c>true</c> if the request deserializes and <c>Equals</c> the objectValue; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   objectValue
        /// </exception>
        public static bool IsJson<T>(this HttpContent content, T objectValue, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (objectValue == null) throw new ArgumentNullException(nameof(objectValue));

            return content.IsJson<T>(x => objectValue.Equals(x), jsonSerializerSettings);
        }

        /// <summary>
        /// Determines whether the content contains the specified json object.
        /// </summary>
        /// <typeparam name="T">The type of the object the content is compared against.</typeparam>
        /// <param name="content">The http content.</param>
        /// <param name="predicate">The predicate that matches the content.</param>
        /// <param name="jsonSerializerSettings">The settings to use when deserializing.</param>
        /// <returns>
        ///   <c>true</c> if the content deserializes and passes the <paramref name="predicate"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   objectValue
        /// </exception>
        public static bool IsJson<T>(this HttpContent content, Func<T, bool> predicate, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (content == null) 
                return false;

            T deserializedObj;
            try
            {
                deserializedObj = content.ReadAsJson<T>(jsonSerializerSettings).GetAwaiter().GetResult();
            }
            catch (JsonReaderException)
            {
                return false;
            }

            if (deserializedObj == null) 
                return false;

            return predicate(deserializedObj);
        }

        /// <summary>
        /// Determines whether the content contains the specified byte array.
        /// </summary>
        /// <param name="content">The http content.</param>
        /// <param name="byteValue">The byte array the content is compared against.</param>
        /// <returns>
        ///   <c>true</c> if the content is the same as <paramref name="byteValue"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// byteValue
        /// </exception>
        public static bool IsByteArray(this HttpContent content, byte[] byteValue)
        {
            if (byteValue == null) throw new ArgumentNullException(nameof(byteValue));
            if (content == null) 
                return false;

            var bytes = content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            if (byteValue.Length != bytes.Length)
                return false;

            for (var i = 0; i < byteValue.Length; i++)
            {
                if (byteValue[i] != bytes[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Clones the http content.
        /// </summary>
        /// <param name="content">The http content to cline.</param>
        /// <returns>A new HttpContent with the same content as the original</returns>
        public static HttpContent Clone(this HttpContent content)
        {
            var ms = new MemoryStream();
            content.CopyToAsync(ms).GetAwaiter().GetResult();
            ms.Position = 0;

            var newContent = new StreamContent(ms);

            foreach (var v in content.Headers)
                newContent.Headers.TryAddWithoutValidation(v.Key, v.Value);

            return newContent;
        }
    }
}
