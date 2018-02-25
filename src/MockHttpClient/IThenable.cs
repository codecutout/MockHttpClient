using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MockHttpClient
{
    /// <summary>
    /// Represents are rule which the action can be set
    /// </summary>
    public interface IThenable : IWhenable
    {
        /// <summary>
        /// Creates the action to run when the rule's condition is met.
        /// </summary>
        /// <param name="resultFactory">The result factory.</param>
        void Then(Func<HttpRequestMessage, Task<HttpResponseMessage>> resultFactory);
    }

    /// <summary>
    /// Provides extension methods on <see cref="IThenable"/>.
    /// </summary>
    public static class IThenableExtensions
    {
        /// <summary>
        /// Creates the action to run when the rule's condition is met.
        /// </summary>
        /// <param name="self">The IThenable.</param>
        /// <param name="resultFactory">The result factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// self
        /// or
        /// resultFactory
        /// </exception>
        public static void Then(this IThenable self, Func<HttpRequestMessage, HttpResponseMessage> resultFactory)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (resultFactory == null) throw new ArgumentNullException(nameof(resultFactory));

            self.Then(x => Task.Run(()=> resultFactory(x)));
        }

        /// <summary>
        /// Creates the a response with the specified status code when the rule's condition is met.
        /// </summary>
        /// <param name="self">The IThenable.</param>
        /// <param name="statusCode">The result factory.</param>
        /// <exception cref="System.ArgumentNullException">self</exception>
        public static void Then(this IThenable self, HttpStatusCode statusCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.Then(x => new HttpResponseMessage(statusCode));
        }
    }


}
