using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MockHttpClient
{
    /// <summary>
    /// Provides http handling based on configurable rules.
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    /// <seealso cref="IWhenable" />
    public class MockHttpMessageHandler : DelegatingHandler, IWhenable
    {
        private class Rule : IWhenable, IThenable
        {
            public Func<HttpRequestMessage, bool> Condition { get; set; }

            public Func<HttpRequestMessage, Task<HttpResponseMessage>> ResponseFactory { get; set; }

            public bool IsRuleComplete => Condition != null && ResponseFactory != null;

            public IThenable When(Func<HttpRequestMessage, bool> condition)
            {
                var oldCondition = Condition;
                Condition = req => oldCondition(req) && condition(req);

                return this;
            }

            public void Then(Func<HttpRequestMessage, Task<HttpResponseMessage>> resultFactory)
            {
                ResponseFactory = resultFactory;
            }
        }

        private Stack<Rule> _rules { get; set; } = new Stack<Rule>();

        private ReaderWriterLockSlim _rulesLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="fallbackHandler">The handler to use when no rules are matched.</param>
        public MockHttpMessageHandler(HttpMessageHandler fallbackHandler)
            :base(fallbackHandler)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        public MockHttpMessageHandler()
         : this(new NotFoundMessageHandler())
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
            Rule rule;
            _rulesLock.EnterReadLock();
            try
            {
                rule = _rules.FirstOrDefault(x => x.IsRuleComplete && x.Condition(request));
            }
            finally
            {
                _rulesLock.ExitReadLock();
            }

            return rule != null
                ? rule.ResponseFactory(request)
                : base.SendAsync(request, cancellationToken);

        }

        /// <summary>
        /// Starts creating a rule by specifying the condition when this rule should be applied.
        /// </summary>
        /// <param name="condition">The condition when the rule should be applied.</param>
        /// <returns></returns>
        public IThenable When(Func<HttpRequestMessage, bool> condition)
        {
            var rule = new Rule()
            {
                Condition = condition
            };
            _rulesLock.EnterWriteLock();
            try
            {
                this._rules.Push(rule);
            }
            finally
            {
                _rulesLock.ExitWriteLock();
            }
            return rule;
        }
    }


}
