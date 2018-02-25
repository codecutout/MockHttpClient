using MockHttpClient.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace MockHttpClient
{
    /// <summary>
    /// Provides extension methods on <see cref="HttpResponseMessage"/>
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        private static readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> EmptyHeader = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();

        /// <summary>
        /// Copies the <see cref="HttpResponseMessage"/> and adds the specified header.
        /// Depending on the header type it will be added to either the response or the content
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValues">The header values.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// response
        /// or
        /// headerName
        /// or
        /// headerValues
        /// </exception>
        /// <exception cref="InvalidOperationException">Unable to add the specific header to the response or content</exception>
        public static HttpResponseMessage WithHeader(this HttpResponseMessage response, string headerName, params string[] headerValues)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (headerName == null) throw new ArgumentNullException(nameof(headerName));
            if (headerValues == null) throw new ArgumentNullException(nameof(headerValues));

            var clone = response.Clone();

            //try add it to the main body
            if (clone.Headers.TryAddWithoutValidation(headerName, headerValues))
                return clone;

            //there are a bunch of headers that are only allowed on the content
            //we will create an empty content and try to add them there
            var content = clone.Content?.Clone() ?? new EmptyContent();
            if(content.Headers.TryAddWithoutValidation(headerName, headerValues))
            {
                clone.Content = content;
                return clone;
            }

            //if we cant add them to the body or content then something is very wrong
            //with the header, just fail out and let the caller work out what htey did wrong
            throw new InvalidOperationException($"Header '{headerName}' with value '{string.Join(",",headerValues)}' can not be added to the response");
        }

        /// <summary>
        /// Copies the <see cref="HttpResponseMessage"/> and adds the specified string content.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="content">The content.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// response
        /// or
        /// content
        /// </exception>
        public static HttpResponseMessage WithStringContent(
            this HttpResponseMessage response, 
            string content, 
            Encoding encoding = null, 
            string mediaType = null)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (content == null) throw new ArgumentNullException(nameof(content));

            var newContent = new StringContent(content, encoding, mediaType);

            foreach (var v in response.Content?.Headers ?? EmptyHeader)
                newContent.Headers.TryAddWithoutValidation(v.Key, v.Value);

            var clone = response.Clone();
            clone.Content = newContent;

            return clone;
        }

        /// <summary>
        /// Copies the <see cref="HttpResponseMessage"/> and adds the specified object serialized as xml string content.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="obj">The object.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">response</exception>
        public static HttpResponseMessage WithXmlContent(
            this HttpResponseMessage response,
            object obj,
            string mediaType = null)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var newContent = new XmlContent(obj, mediaType);
            
            foreach (var v in response.Content?.Headers ?? EmptyHeader)
                newContent.Headers.TryAddWithoutValidation(v.Key, v.Value);

            var clone = response.Clone();
            clone.Content = newContent;

            return clone;
        }

        /// <summary>
        /// Copies the <see cref="HttpResponseMessage"/> and adds the specified object serialized as json string content.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="obj">The object.</param>
        /// <param name="mediaType">The media type.</param>
        /// <param name="settings">The json serializer settings.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">response</exception>
        public static HttpResponseMessage WithJsonContent(
            this HttpResponseMessage response, 
            object obj, 
            string mediaType = null, 
            JsonSerializerSettings settings = null)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var newContent = new JsonContent(obj, mediaType, settings);

            foreach (var v in response.Content?.Headers ?? EmptyHeader)
                newContent.Headers.TryAddWithoutValidation(v.Key, v.Value);

            var clone = response.Clone();
            clone.Content = newContent;

            return clone;
        }

        /// <summary>
        /// Shallow clones the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static HttpResponseMessage Clone(this HttpResponseMessage response)
        {
            var clone = new HttpResponseMessage();
            clone.StatusCode = response.StatusCode;
            clone.ReasonPhrase = response.ReasonPhrase;
            clone.Content = response.Content;
            clone.Version = response.Version;

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

       
    }


}
