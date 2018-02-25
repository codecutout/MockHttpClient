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
    public class TestWithJsonContent
    {
        [Fact]
        public async Task should_create_content()
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithJsonContent(new
                {
                    FirstName = "John",
                    LastName = "Doe",
                }));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);

            var responseObj = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal(responseObj.FirstName.ToString(), "John");
            Assert.Equal(responseObj.LastName.ToString(), "Doe");

            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task should_create_content_with_media_type()
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithJsonContent(new
                {
                    FirstName = "John",
                    LastName = "Doe",
                }, "application/vnd.mockhttphandler.local"));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);

            var responseObj = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal(responseObj.FirstName.ToString(), "John");
            Assert.Equal(responseObj.LastName.ToString(), "Doe");

            Assert.Equal("application/vnd.mockhttphandler.local", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task should_create_content_with_media_type_and_serializer_settings()
        {
            var client = new MockHttpClient();

            client.When("*").Then(x => new HttpResponseMessage()
                .WithJsonContent(new
                {
                    FirstName = "John",
                    LastName = "Doe",
                }, "application/vnd.mockhttphandler.local", new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://mockhttphandler.local");

            var response = await client.SendAsync(request);

            var responseObj = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal(responseObj.firstName.ToString(), "John");
            Assert.Equal(responseObj.lastName.ToString(), "Doe");

            Assert.Equal("application/vnd.mockhttphandler.local", response.Content.Headers.ContentType.MediaType);
        }


    }
}
