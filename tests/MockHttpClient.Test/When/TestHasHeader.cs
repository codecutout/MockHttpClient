using MockHttpClient.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    public class TestHasHeader
    {
        [Fact]
        public async Task When_header_condition_should_match()
        {
            var client = new MockHttpClient();

            client
                .When(x=>x.HasHeader("version", "1"))
                .Then(HttpStatusCode.OK);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");
            request.Headers.Add("version", "1");

            Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(request)).StatusCode);
        }

        [Fact]
        public async Task When_header_wihtout_value_should_match_any_value()
        {
            var client = new MockHttpClient();

            client.When(x => x.HasHeader("version"))
                .Then(HttpStatusCode.OK);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");
            request.Headers.Add("version", "4");

            Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(request)).StatusCode);
        }

        [Fact]
        public async Task When_multiple_headers_condition_should_match_any()
        {
            var client = new MockHttpClient();

            client.When(x => x.HasHeader("Accept", "application/json"))
                .Then(HttpStatusCode.OK);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");
            request.Headers.Add("Accept", "application/xml");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Accept", "text/html");

            Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(request)).StatusCode);
        }

        [Fact]
        public async Task When_content_header_condition_should_match()
        {
            var client = new MockHttpClient();

            client.When(x => x.HasHeader("content-type", "application/json"))
                .Then(HttpStatusCode.OK);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");
            request.Content = new StringContent("");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(request)).StatusCode);
        }


    }
}
