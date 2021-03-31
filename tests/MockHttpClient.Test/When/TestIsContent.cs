using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    using System.Net.Http.Json;
    using Newtonsoft.Json;

    public class TestIsContent
    {

        [Fact]
        public async Task When_string_content_same_should_match()
        {
            // Arrange
            var client = new MockHttpClient();

            var content = "name=Wayne";

            client
                .When(x => x.Content.IsString(content))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"),
                new StringContent(content));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_string_content_different_should_not_match()
        {
            // Arrange
            var client = new MockHttpClient();

            client
                .When(x => x.Content.IsString("name=Wayne"))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.GetAsync(new Uri("http://mockhttphandler.local"));

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_content_same_json_should_match()
        {
            // Arrange
            var testDto = new TestDto
            {
                Firstname = "Wayne",
                IsMale = true
            };

            var client = new MockHttpClient();

            client
                .When(x => x.Content.IsJson(testDto))
                .Then(HttpStatusCode.OK);

            var content = JsonConvert.SerializeObject(testDto);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new StringContent(content));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_content_same_json_as_anonymous_should_match()
        {
            // Arrange
            var client = new MockHttpClient();
            client
                .When(x => x.Content.IsJson(new
                {
                    FirstName = "Wayne",
                }))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new StringContent(@"{""Firstname"":""Wayne"", ""IsMale"":true}"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_content_not_json_should_not_match()
        {
            // Arrange
            var client = new MockHttpClient();

            client
                .When(x => x.Content.IsJson<TestDto>(x=>true))
                .Then(HttpStatusCode.OK);


            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new StringContent("not-a-json-string"));

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_json_predicate_pass_then_should_match()
        {
            // Arrange
            var client = new MockHttpClient();
            client
                .When(x => x.Content.IsJson<TestDto>(i => i.Firstname == "Wayne" && i.IsMale == true))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsJsonAsync(new Uri("http://mockhttphandler.local"),
                new TestDto
                {
                    Firstname = "Wayne",
                    IsMale = true
                });

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_byte_array_same_should_match()
        {
            // Arrange
            var client = new MockHttpClient();
            client
                .When(x => x.Content.IsByteArray(new byte[] { 10, 20, 30 }))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new ByteArrayContent(new byte[] { 10, 20, 30 }));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task When_byte_array_different_should_not_match()
        {
            // Arrange
            var client = new MockHttpClient();
            client
                .When(x => x.Content.IsByteArray(new byte[] { 11, 22, 33 }))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new ByteArrayContent(new byte[] { 10, 20, 30 }));

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }

    public class TestDto : IEquatable<TestDto>
    {
        public string Firstname { get; set; }

        public bool IsMale { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestDto);
        }
        
        public bool Equals(TestDto other)
        {
            return Firstname == other.Firstname 
                   && IsMale == other.IsMale;
        }

        public override int GetHashCode()
        {
            return new { Firstname, IsMale }.GetHashCode();
        }
    }
}