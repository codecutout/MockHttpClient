using System;
using System.Net.Http;
namespace MockHttpClient
{
    /// <summary>
    /// Represents an object which can start having rules defined against it
    /// </summary>
    public interface IWhenable
    {
        /// <summary>
        /// Starts creating a rule by specifying the condition when this rule should be applied.
        /// </summary>
        /// <param name="condition">The condition when the rule should be applied.</param>
        /// <returns></returns>
        IThenable When(Func<HttpRequestMessage, bool> condition);
    }

    /// <summary>
    /// Provides extension methods on <see cref="IWhenable"/>
    /// </summary>
    public static class IWhenableExtensions
    {
        /// <summary>
        /// Creates a rule that is run when the request url matches the specified url.
        /// </summary>
        /// <param name="self">The IWhenable.</param>
        /// <param name="url">The url query in the format accpeted by <see cref="UrlQuery"/>.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// self
        /// or
        /// url
        /// </exception>
        /// <exception cref="Exceptions.InvalidUrlQueryException">The url is not a valid format for <see cref="UrlQuery"/>.</exception>
        public static IThenable When(this IWhenable self, string url)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (url == null) throw new ArgumentNullException(nameof(url));

            //generate the urlQuery here so we can throw if they give an invalid string
            var urlQuery = new UrlQuery(url);
            return self.When(x => x.HasUrl(urlQuery));
        }

        /// <summary>
        /// Creates a rule that is run when the request url matches the specified url.
        /// </summary>
        /// <param name="self">The IWhenable.</param>
        /// <param name="url">The url to compare with an exact match.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// self
        /// or
        /// url
        /// </exception>
        public static IThenable When(this IWhenable self, Uri url)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (url == null) throw new ArgumentNullException(nameof(url));

            return self.When(x => x.HasUrl(url));
        }

        /// <summary>
        ///  Creates a rule that is run when the request url matches the specified method and url.
        /// </summary>
        /// <param name="self">The IWhenable.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The url query in the format accpeted by <see cref="UrlQuery"/>.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// self
        /// or
        /// method
        /// or
        /// url
        /// </exception>
        /// <exception cref="Exceptions.InvalidUrlQueryException">The url is not a valid format for <see cref="UrlQuery"/>.</exception>
        public static IThenable When(this IWhenable self, HttpMethod method, string url)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (url == null) throw new ArgumentNullException(nameof(url));

            //generate the urlQuery here so we can throw if they give an invalid string
            var urlQuery = new UrlQuery(url);
            return self.When(x => x.HasMethod(method) && x.HasUrl(urlQuery));
        }

        /// <summary>
        ///  Creates a rule that is run when the request url matches the specified method and url.
        /// </summary>
        /// <param name="self">The IWhenable.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The url to compare with an exact match.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// self
        /// or
        /// method
        /// or
        /// url
        /// </exception>
        public static IThenable When(this IWhenable self, HttpMethod method, Uri url)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (url == null) throw new ArgumentNullException(nameof(url));

            return self.When(x => x.HasMethod(method) && x.HasUrl(url));
        }

      
    }


}
