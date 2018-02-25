using MockHttpClient.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    public class TestHistory
    {
        [Fact]
        public async Task Should_record_request_history()
        {
            var client = new MockHttpClient();

            client.When("*").Then(HttpStatusCode.OK);

            
            await client.GetAsync("http://mockhttphandler.local/1");
            await client.GetAsync("http://mockhttphandler.local/2");
            await client.GetAsync("http://mockhttphandler.local/3");

            Assert.Equal(3, client.RequestHistory.Count);
            Assert.Equal("/1", client.RequestHistory[0].RequestUri.AbsolutePath);
            Assert.Equal("/2", client.RequestHistory[1].RequestUri.AbsolutePath);
            Assert.Equal("/3", client.RequestHistory[2].RequestUri.AbsolutePath);

        }

    }
}
