using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MockHttpClient
{
    /// <summary>
    /// Provides 404 Not Found default handling
    /// </summary>
    /// <seealso cref="System.Net.Http.HttpMessageHandler" />
    public class NotFoundMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //using Task.Run to simulate a requset not being instant
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
        }
    }


}
