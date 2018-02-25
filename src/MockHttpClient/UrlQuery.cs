using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MockHttpClient.Exceptions;

namespace MockHttpClient
{
    /// <summary>
    /// Provides fuzzy matching against a Uri
    /// </summary>
    public class UrlQuery
    {
        private static Regex AcceptEverythingRegex = new Regex("");
        private readonly Regex _schemeRegex = AcceptEverythingRegex;
        private readonly Regex _hostRegex = AcceptEverythingRegex;
        private readonly Regex _pathRegex = AcceptEverythingRegex;
        private readonly ILookup<string, Regex> _queryStringRegex = Enumerable.Empty<Regex>().ToLookup(x => default(string));

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlQuery"/> class.
        /// </summary>
        /// <param name="urlQuery">The URL query.</param>
        /// <exception cref="System.ArgumentNullException">urlQuery</exception>
        /// <exception cref="InvalidUrlQueryException"></exception>
        /// <example>
        /// <code>new UrlQuery("*");</code>
        /// <code>new UrlQuery("http://urlquery.local/my/resource/path");</code>
        /// <code>new UrlQuery("//urlquery.local/my/resource/path");</code>
        /// <code>new UrlQuery("/my/resource/path");</code>
        /// <code>new UrlQuery("?key=true");</code>
        /// </example>
        public UrlQuery(string urlQuery)
        {
            if (urlQuery == null) throw new ArgumentNullException(nameof(urlQuery));

            if(urlQuery == "" || urlQuery == "*")
                return; //keep the default of accept everything

            var parsed = false;

            //abosulte url
            var schemeIndex = urlQuery.IndexOf("://");
            if (schemeIndex >= 0)
            {
                parsed = true;
                var scheme = urlQuery.Substring(0, schemeIndex);
                _schemeRegex = WildcardToRegex(scheme, options: RegexOptions.IgnoreCase);
                urlQuery = urlQuery.Substring(schemeIndex + 1);
            }

            //scheme relative
            if (urlQuery.StartsWith("//"))
            {
                parsed = true;
                var hostIndex = urlQuery.IndexOf("/", 2);
                hostIndex = hostIndex < 0 ? urlQuery.Length : hostIndex;
                var hostLength = hostIndex < 0
                    ? urlQuery.Length - 2
                    : hostIndex - 2;
                var host = urlQuery.Substring(2, hostLength);
                _hostRegex = WildcardToRegex(host, options: RegexOptions.IgnoreCase);
                urlQuery = urlQuery.Substring(hostIndex);
            }

            //relative
            if (urlQuery.StartsWith("/"))
            {
                parsed = true;
                var queryStringIndex = urlQuery.IndexOf('?');
                var pathLength = queryStringIndex < 0
                    ? urlQuery.Length
                    : queryStringIndex;
                var path = urlQuery.Substring(0, pathLength);
                _pathRegex = WildcardToRegex(path.TrimEnd('/'), regexSuffix: "/?", options: RegexOptions.IgnoreCase);

                urlQuery = urlQuery.Substring(pathLength);
            }

            //query string
            if (urlQuery.StartsWith("?"))
            {
                parsed = true;
                var queryParameters = ParseQueryString(urlQuery);
                _queryStringRegex = queryParameters.ToLookup(x=>x.Key, x=> WildcardToRegex(x.Value));
            }

            if (!parsed)
                throw new InvalidUrlQueryException(urlQuery);
        }

        /// <summary>
        /// Determines if the UrlQuery matches the specified Uri
        /// </summary>
        /// <param name="uri">The Uri to match against.</param>
        /// <returns><c>true</c> if the UrlQuery matches the Uri; otherwise <c>false</c></returns>
        public bool Match(Uri uri)
        {
            if (!_schemeRegex.IsMatch(uri.Scheme)
                || !_hostRegex.IsMatch(uri.Host)
                || !_pathRegex.IsMatch(uri.AbsolutePath))
                return false;

            var requestQuery = ParseQueryString(uri.Query).ToLookup(x=>x.Key, x=>x.Value);
            foreach(var requiredValues in _queryStringRegex)
            {
                //request must have at least one of the required values
                if (!requestQuery.Contains(requiredValues.Key))
                    return false;

                //when there are mutliple values their must be a matching entry for each required value
                var requestParameters = new List<string>(requestQuery[requiredValues.Key]);
                foreach(var requiredValue in requiredValues)
                {
                    var matchedRequestParameter = requestParameters.FirstOrDefault(x => requiredValue.IsMatch(x));
                    if (matchedRequestParameter == null)
                        return false;
                    requestParameters.Remove(matchedRequestParameter);
                }
            }

            return true;
        }

        private static Regex WildcardToRegex(string pattern, string regexSuffix = "", RegexOptions options = RegexOptions.None)
        {
            return new Regex("^" + 
                Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") +
                regexSuffix +
                "$",
                options);
        }

        private static IEnumerable<KeyValuePair<string,string>> ParseQueryString(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return Enumerable.Empty<KeyValuePair<string, string>>();

            return queryString.TrimStart('?').Split('&').Select(q =>
            {
                var equalIndex = q.IndexOf('=');
                var key = Uri.UnescapeDataString(q.Substring(0, equalIndex));
                var value = Uri.UnescapeDataString(q.Substring(equalIndex + 1));
                return new KeyValuePair<string, string>(key, value);
            });
        }
    }
}
