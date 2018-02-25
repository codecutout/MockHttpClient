using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MockHttpClient
{
    /// <summary>
    /// Provides logging of past requests
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class RequestHistoryHttpMessageHandler : DelegatingHandler
    {
        private List<HttpRequestMessage> _history = new List<HttpRequestMessage>();

        /// <summary>
        /// Gets the request history.
        /// </summary>
        public IReadOnlyList<HttpRequestMessage> History => _history;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHistoryHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public RequestHistoryHttpMessageHandler(HttpMessageHandler innerHandler)
            :base(innerHandler)
        {

        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _history.Add(request);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
