using MockHttpClient.Content;
using MockHttpClient.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    public class TestHttpRepository
    {
        public class Widget
        {
            public string Name { get; set; }

            public int Rating { get; set; }
        }

        public readonly HttpClient Repo;
        public TestHttpRepository()
        {
            var repo = new MockHttpClient();

            repo.When(putRequest => putRequest.HasMethod(HttpMethod.Put))
                .Then(async putRequest =>
                {
                    var requestString = await putRequest.Content.ReadAsStringAsync();
                    repo.When(HttpMethod.Get, putRequest.RequestUri)
                        .Then(x => new HttpResponseMessage(HttpStatusCode.OK)
                            .WithStringContent(
                                requestString, 
                                mediaType: putRequest.Content.Headers.ContentType.MediaType));
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            repo.BaseAddress = new Uri("http://mockhttpclient.local");
            Repo = repo;
        }

        [Fact]
        public async Task should_fetch_posted_content()
        {
            await Repo.PutAsync("http://mockhttpclient.local/widget/1", new JsonContent(new Widget()
            {
                Name = "Foo",
                Rating = 10
            }));

            var widget1Response = await Repo.GetAsync("/widget/1");
            var widget1 = await widget1Response.Content.ReadAsJson<Widget>();
            Assert.Equal(HttpStatusCode.OK, widget1Response.StatusCode);
            Assert.Equal("Foo", widget1.Name);
            Assert.Equal(10, widget1.Rating);
        }

        [Fact]
        public async Task should_fetch_latest_posted_content()
        {

            await Repo.PutAsync("http://mockhttpclient.local/widget/1", new JsonContent(new Widget()
            {
                Name = "Foo",
                Rating = 15
            }));

            await Repo.PutAsync("http://mockhttpclient.local/widget/1", new JsonContent(new Widget()
            {
                Name = "Foo-2",
                Rating = 25
            }));

            var widget1Response = await Repo.GetAsync("/widget/1");
            var widget1 = await widget1Response.Content.ReadAsJson<Widget>();
            Assert.Equal(HttpStatusCode.OK, widget1Response.StatusCode);
            Assert.Equal("Foo-2", widget1.Name);
            Assert.Equal(25, widget1.Rating);
        }

        [Fact]
        public async Task should_store_multiple()
        {

            await Repo.PutAsync("http://mockhttpclient.local/widget/1", new JsonContent(new Widget()
            {
                Name = "Foo",
                Rating = 15
            }));

            await Repo.PutAsync("http://mockhttpclient.local/widget/2", new JsonContent(new Widget()
            {
                Name = "Bar",
                Rating = 25
            }));

            await Repo.PutAsync("http://mockhttpclient.local/widget/3", new JsonContent(new Widget()
            {
                Name = "Zut",
                Rating = 10
            }));

            var widget1Response = await Repo.GetAsync("/widget/1");
            var widget1 = await widget1Response.Content.ReadAsJson<Widget>();
            Assert.Equal(HttpStatusCode.OK, widget1Response.StatusCode);
            Assert.Equal("Foo", widget1.Name);
            Assert.Equal(15, widget1.Rating);

            var widget2Response = await Repo.GetAsync("/widget/2");
            var widget2 = await widget2Response.Content.ReadAsJson<Widget>();
            Assert.Equal(HttpStatusCode.OK, widget2Response.StatusCode);
            Assert.Equal("Bar", widget2.Name);
            Assert.Equal(25, widget2.Rating);

            var widget3Response = await Repo.GetAsync("/widget/3");
            var widget3 = await widget3Response.Content.ReadAsJson<Widget>();
            Assert.Equal(HttpStatusCode.OK, widget3Response.StatusCode);
            Assert.Equal("Zut", widget3.Name);
            Assert.Equal(10, widget3.Rating);
        }



    }
}
