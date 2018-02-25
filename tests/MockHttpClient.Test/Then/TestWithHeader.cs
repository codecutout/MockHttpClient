using MockHttpClient.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    public class TestWithHeader
    {
        [Theory]
        [InlineData("trailer", "expires")]
        [InlineData("transfer-encoding", "gzip")]
        [InlineData("vary", "Version")]
        [InlineData("via", "HTTP/1.1 GWA")]
        [InlineData("upgrade", "HTTP/1.1 GWA")]
        [InlineData("warning", "110")]
        [InlineData("www-authenticate", "basic")]
        [InlineData("my-custom-header", "BOOM")]
        public async Task should_add_header(string header, string value)
        {
            var client = new MockHttpClient();

            client.When("*").Then(x=>new HttpResponseMessage()
                .WithHeader(header, value));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);
            var headers = response.Headers. GetValues(header).ToList();

            Assert.Single(headers);
            Assert.Equal(value, headers[0]);
        }

        [Theory]
        [InlineData("content-disposition", "inline")]
        [InlineData("content-encoding", "utf-8")]
        [InlineData("content-language", "en_US")]
        [InlineData("content-length", "0")]
        [InlineData("content-location", "http://mockhttphandler.local/location")]
        [InlineData("content-md5", "bXkgbWQ1")]
        [InlineData("content-range", "1024-2048")]
        [InlineData("content-type", "application/json")]
        [InlineData("expires", "2112-12-12")]
        [InlineData("last-modified", "2112-12-12")]
        public async Task should_add_content_headers(string header, string value)
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithHeader(header, value));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);
            var headers = response.Content.Headers.GetValues(header).ToList();
            Assert.Single(headers);
            Assert.Equal(value, headers[0]);
        }

        [Fact]
        public async Task should_add_header_mix()
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithHeader("content-disposition", "inline")
                .WithHeader("warning", "110")
                .WithHeader("content-type", "application/json")
                .WithHeader("my-custom-header", "BOOM"));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);

            Assert.Equal("inline", response.Content.Headers.GetValues("content-disposition").Single());
            Assert.Equal("application/json", response.Content.Headers.GetValues("content-type").Single());
            Assert.Equal("110", response.Headers.GetValues("warning").Single());
            Assert.Equal("BOOM", response.Headers.GetValues("my-custom-header").Single());
        }


    }
}
