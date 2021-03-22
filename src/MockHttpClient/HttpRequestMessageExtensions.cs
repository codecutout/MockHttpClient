using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace MockHttpClient
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides extension methods on <see cref="HttpRequestMessage"/>
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Determines whether the request uses the specified method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="method">The method.</param>
        /// <returns>
        ///   <c>true</c> if the request uses the specified method; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// method
        /// </exception>
        public static bool HasMethod(this HttpRequestMessage request, HttpMethod method)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (method == null) throw new ArgumentNullException(nameof(method));

            return request.Method == method;
        }

        /// <summary>
        /// Determines whether the request matches the url.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="url">The url query in the format accpeted by <see cref="UrlQuery"/>.</param>
        /// <returns>
        ///   <c>true</c> if the requset uses the specified url; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// url
        /// </exception>
        /// <exception cref="Exceptions.InvalidUrlQueryException">The url is not a valid format for <see cref="UrlQuery"/>.</exception>
        public static bool HasUrl(this HttpRequestMessage request, string url)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (url == null) throw new ArgumentNullException(nameof(url));

            return HasUrl(request, new UrlQuery(url));
        }

        /// <summary>
        /// Determines whether the request matches the url.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="urlQuery">The url query.</param>
        /// <returns>
        ///   <c>true</c> if the requset uses the specified url; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// urlQuery
        /// </exception>
        public static bool HasUrl(this HttpRequestMessage request, UrlQuery urlQuery)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (urlQuery == null) throw new ArgumentNullException(nameof(urlQuery));

            return urlQuery.Match(request.RequestUri);
        }

        /// <summary>
        /// Determines whether the request matches the url.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="url">The url to compare with an exact match.</param>
        /// <returns>
        ///   <c>true</c> if the requset uses the specified url; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// url
        /// </exception>
        public static bool HasUrl(this HttpRequestMessage request, Uri url)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (url == null) throw new ArgumentNullException(nameof(url));

            return url.IsAbsoluteUri
                ? request.RequestUri.Equals(url)
                : new Uri(request.RequestUri.PathAndQuery, UriKind.Relative).Equals(url);
        }

        /// <summary>
        /// Determines whether the the request contains the specified header.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>
        ///   <c>true</c> if the request contains the specified header; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// headerName
        /// </exception>
        public static bool HasHeader(this HttpRequestMessage request, string headerName, string headerValue = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (headerName == null) throw new ArgumentNullException(nameof(headerName));

            IEnumerable<string> values;

            //check normal header
            if (request.Headers.TryGetValues(headerName, out values)
                && (headerValue == null || values.Any(x => x == headerValue)))
                return true;

            //check content header
            if (request.Content != null && request.Content.Headers.TryGetValues(headerName, out values)
                && (headerValue == null || values.Any(x => x == headerValue)))
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the request contains the specified content as string.
        /// </summary>
        /// <param name="request">the request</param>
        /// <param name="expectedContent">the expected content</param>
        /// <returns></returns>
        public static bool HasStringContent(this HttpRequestMessage request, string expectedContent)
        {
            if (request.Content == null)
            {
                throw new ArgumentException($"string content '{expectedContent}' was expected but content is null");
            }

            var content = request.Content.ReadAsStringAsync().Result;

            return content.Equals(expectedContent);
        }

        /// <summary>
        /// Determines whether the request contains the specified content as json object.
        /// </summary>
        /// <typeparam name="T">The type of the expected content</typeparam>
        /// <param name="request">the request</param>
        /// <param name="expectedContent">the expected content</param>
        /// <returns></returns>
        public static bool HasJsonContent<T>(this HttpRequestMessage request, T expectedContent)
        {
            if (request.Content == null)
            {
                throw new ArgumentException($"json content '{expectedContent}' was expected but content is null");
            }

            var stringContent = request.Content.ReadAsStringAsync().Result;
            var content = JsonConvert.DeserializeObject<T>(stringContent);

            if (content == null)
            {
                throw new ArgumentException($"could not deserialize content to type {typeof(T)}");
            }

            return expectedContent.Equals(content);
        }

        /// <summary>
        /// Determines whether the request matches the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of the expected content</typeparam>
        /// <param name="request">the request</param>
        /// <param name="predicate">the predicate that matches the content</param>
        /// <returns></returns>
        public static bool HasJsonContent<T>(this HttpRequestMessage request, Predicate<T> predicate)
        {
            if (request.Content == null)
            {
                throw new ArgumentException("content is null");
            }

            var stringContent = request.Content.ReadAsStringAsync().Result;
            var content = JsonConvert.DeserializeObject<T>(stringContent);

            if (content == null)
            {
                throw new ArgumentException($"could not deserialize content to type {typeof(T)}");
            }

            return predicate.Invoke(content);
        }

        /// <summary>
        /// Determines whether the request contains the specified content as json object.
        /// </summary>
        /// <param name="request">the request</param>
        /// <param name="expectedContent">the expected content</param>
        /// <returns></returns> 
        public static bool HasByteArrayContent(this HttpRequestMessage request, byte[] expectedContent)
        {
            if (request.Content == null)
            {
                throw new ArgumentException("content is null");
            }

            var content = request.Content.ReadAsByteArrayAsync().Result;

            if (expectedContent.Length != content.Length)
            {
                return false;
            }

            for (var i = 0; i < expectedContent.Length; i++)
            {
                if (expectedContent[i] != content[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
