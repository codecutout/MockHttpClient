using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockHttpClient.Test
{
    using System.Net.Http.Json;
    using Newtonsoft.Json;

    public class TestHasContent
    {
        [Fact]
        public void ShouldThrowArgumentExceptionIfStringContentIsNotSet()
        {
            // Arrange
            var client = new MockHttpClient();

            client
                .When(x => x.HasStringContent("name=Wayne"))
                .Then(HttpStatusCode.OK);

            // Act
            Func<Task> act = async () => await client.GetAsync(new Uri("http://mockhttphandler.local"));

            // Assert
            Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionIfJsonContentIsNotSet()
        {
            // Arrange
            var client = new MockHttpClient();

            client
                .When(x => x.HasJsonContent("name=Wayne"))
                .Then(HttpStatusCode.OK);


            // Act
            Func<Task> act = async () => await client.GetAsync(new Uri("http://mockhttphandler.local"));

            // Assert
            Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionIfJsonContentIsNotSetWhileUsingPredicate()
        {
            // Arrange
            var client = new MockHttpClient();

            client
                .When(req => req.HasJsonContent<TestDto>(i => true))
                .Then(HttpStatusCode.OK);

            // Act
            Func<Task> act = async () => await client.GetAsync(new Uri("http://mockhttphandler.local"));

            // Assert
            Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task ShouldReturnOkIfStringContentMatches()
        {
            // Arrange
            var client = new MockHttpClient();

            var content = "name=Wayne";

            client
                .When(x => x.HasStringContent(content))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"),
                new StringContent(content));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkIfJsonContentMatchesExactly()
        {
            // Arrange
            var testDto = new TestDto
            {
                Firstname = "Wayne",
                IsMale = true
            };

            var client = new MockHttpClient();

            client
                .When(x => x.HasJsonContent(testDto))
                .Then(HttpStatusCode.OK);

            var content = JsonConvert.SerializeObject(testDto);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new StringContent(content));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkIfJsonContentMatchesLogically()
        {
            // Arrange
            var testDto = new TestDto
            {
                Firstname = "Wayne",
                IsMale = true
            };

            var client = new MockHttpClient();

            client
                .When(x => x.HasJsonContent<TestDto>(i => i.Firstname == testDto.Firstname
                                                          && i.IsMale == testDto.IsMale))
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
        public async Task ShouldReturnOkIfByteArrayContentMatches()
        {
            // Arrange
            var client = new MockHttpClient();
            var content = new byte[] {10, 20, 30};

            client
                .When(x => x.HasByteArrayContent(content))
                .Then(HttpStatusCode.OK);

            // Act
            var result = await client.PostAsync(new Uri("http://mockhttphandler.local"), new ByteArrayContent(content));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
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
    }
}