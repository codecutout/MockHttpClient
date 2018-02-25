using MockHttpClient.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    public class Name
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class TestWithXmlContent
    {
        [Fact]
        public async Task should_create_content()
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithXmlContent(new Name
                {
                    FirstName = "John",
                    LastName = "Doe",
                }));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("<FirstName>John</FirstName>", result);
            Assert.Contains("<LastName>Doe</LastName>", result);

            Assert.Equal("application/xml", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task should_create_content_with_media_type()
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithXmlContent(new Name
                {
                    FirstName = "John",
                    LastName = "Doe",
                }, "application/vnd.mockhttphandler.local"));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("<FirstName>John</FirstName>", result);
            Assert.Contains("<LastName>Doe</LastName>", result);

            Assert.Equal("application/vnd.mockhttphandler.local", response.Content.Headers.ContentType.MediaType);
        }
    }
}
