using MockHttpClient.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    public class TestHasUrl
    {
        [Theory]
        [InlineData("http://mockhttpclient.local/path/to/resource", "", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "*", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "http://mockhttpclient.local/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "http*://mockhttpclient.local/path/to/resource", true)]
        [InlineData("https://mockhttpclient.local/path/to/resource", "http*://mockhttpclient.local/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "http://*.local/path/to/resource", true)]
        [InlineData("http://mockHTTPclient.local/path/to/resource", "http://MOCKhttpCLIENT.local/path/to/resource", true)]

        [InlineData("http://mockhttpclient.local/path/to/resource", "//mockhttpclient.local/path/to/resource", true)]
        [InlineData("https://mockhttpclient.local/path/to/resource", "//mockhttpclient.local/path/to/resource", true)]

        [InlineData("http://mockhttpclient.local/path/to/resource", "/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "/path/TO/resource/", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "/path/*/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "/path/*", true)]

        [InlineData("http://mockhttpclient.local/path/to/resource", "/path/to/resource/", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource/", "/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource/", "/path/to/resource/", true)]

        [InlineData("http://mockhttpclient.local/path/to/resource?q=test", "/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q=test", "/path/to/resource?q=test", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q=test", "/path/to/resource?q=*", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q=test", "/path/to/resource?q=false", false)]

        [InlineData("http://mockhttpclient.local?q1=a&q2=b", "?q1=a", true)]
        [InlineData("http://mockhttpclient.local/?q1=a&q2=b", "?q1=a", true)]
        [InlineData("http://mockhttpclient.local?q1=a&q2=b", "?q2=b", true)]
        [InlineData("http://mockhttpclient.local?q1=a&q2=b", "?q1=a&q2=b", true)]
        [InlineData("http://mockhttpclient.local?q1=a&q2=b", "?q2=b&q1=a", true)]
        [InlineData("http://mockhttpclient.local?q1=a&q2=b", "?q1=a&q2=a", false)]
        [InlineData("http://mockhttpclient.local?q1=a&q2=b", "?q1=a&q2=b&q3=c", false)]

        [InlineData("http://mockhttpclient.local?q=a&q=b", "?q=a&q=b", true)]
        [InlineData("http://mockhttpclient.local?q=a&q=b", "?q=a&q=a", false)]
        [InlineData("http://mockhttpclient.local?q=a&q=a", "?q=a&q=a", true)]
        [InlineData("http://mockhttpclient.local?q=a&q=a", "?q=a&q=a&q=a", false)]
        [InlineData("http://mockhttpclient.local?q=a&q=a", "?q=a&q=*", true)]
        [InlineData("http://mockhttpclient.local?q1=a&q1=b&q2=a&q2=b", "?q1=a&q1=b&q2=b", true)]
        public async Task When_url_query_condition_should_match(string requestUri, string pathRule, bool match)
        {
            var client = new MockHttpClient();

            client.When(pathRule)
                .Then(HttpStatusCode.OK);

            Assert.Equal(match ? HttpStatusCode.OK : HttpStatusCode.NotFound, (await client.GetAsync(requestUri)).StatusCode);
        }

        [Theory]
        [InlineData("http://mockhttpclient.local/path/to/resource", "http://mockhttpclient.local/path/to/resource", true)]
        [InlineData("http://mockHTTPclient.local/path/to/resource", "http://MOCKhttpCLIENT.local/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource", "/path/to/resource", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q=a", "/path/to/resource?q=a", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q1=a&q2=b", "/path/to/resource?q1=a&q2=b", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q1=a&q2=b", "/path/to/resource?q2=b&q1=a", false)]
        public async Task When_url_condition_should_match(string requestUri, string queryUri, bool match)
        {
            var client = new MockHttpClient();

            client.When(new Uri(queryUri, UriKind.RelativeOrAbsolute))
                .Then(HttpStatusCode.OK);

            Assert.Equal(match ? HttpStatusCode.OK : HttpStatusCode.NotFound, (await client.GetAsync(requestUri)).StatusCode);
        }


        [Theory]
        [InlineData("not-a-relative-url")]
        public async Task When_invalid_urlQuery_should_throw(string urlQuery)
        {
            var client = new MockHttpClient();

            Assert.Throws<InvalidUrlQueryException>(() => client.When(urlQuery).Then(HttpStatusCode.OK));
        }

        [Theory]
        [InlineData("http://mockhttpclient.local/path/to/resource?q1=a&q2=b", "?q1=a", "?q2=b", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q1=a&q2=b", "?q1=a", "?q2=c", false)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q1=a&q2=b", "//mockhttpclient.local", "/path/*", true)]
        [InlineData("http://mockhttpclient.local/path/to/resource?q1=a&q2=b", "https://mockhttpclient.local", "/path/*", false)]
        public async Task When_multiple_conditions_should_match_all(string requestUri, string queryUri1, string queryUri2, bool match)
        {
            var client = new MockHttpClient();

            client.When(queryUri1)
                .When(queryUri2)
                .Then(HttpStatusCode.OK);

            Assert.Equal(match ? HttpStatusCode.OK : HttpStatusCode.NotFound, (await client.GetAsync(requestUri)).StatusCode);
        }

    }
}
