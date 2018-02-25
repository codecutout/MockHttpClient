using System;
using System.Collections.Generic;
using System.Text;

namespace MockHttpClient.Exceptions
{
    /// <summary>
    /// Represents error that occurs when creating a <see cref="UrlQuery"/> with an invalid string.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidUrlQueryException : Exception
    {
        /// <summary>
        /// The invalid query string.
        /// </summary>
        public readonly string UrlQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUrlQueryException"/> class.
        /// </summary>
        /// <param name="urlQuery">The invalid query string.</param>
        public InvalidUrlQueryException(string urlQuery)
            :base($@"UrlQuery of '{urlQuery}' is not in the correct format. 
Try starting the query with either 'http://' for absoulte url, '//' for schema relative url, '/' for relative urls or '?' for query string only queries")
        {
            UrlQuery = urlQuery;
        }
    }
}
