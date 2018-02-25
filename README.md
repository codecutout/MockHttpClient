# MockHttpClient

Easily mock `HttpClient` for testing

```csharp
//Arrange
var mockHttpClient = new MockHttpClient();
mockHttpClient.When("http://www.google.com").Returns(HttpStatusCode.OK)

//Act
var response = await client.GetAsync(requestUri);

//Assert
Assert.Equal(HttpStatusCode.OK, response);
```

[![NuGet version](https://badge.fury.io/nu/MockHttpClient.svg)](https://badge.fury.io/nu/MockHttpClient)

## url matching

You can specify partial and wildcarded urls in the `when` condition to match across many differnt urls. Below are a few example of how to specify url queries

```
mockHttpClient.When([url string]).Returns(HttpStatusCode.OK)
```

| url string                                     | matches																							 |
| ---------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| `http://mockhttpclient.local/path/to/resource` | matches full Url																					 |
| `http://mockhttpclient.local/path/to/*`        | matches paths starting with `/path/to`															 |
| `http://mockhttpclient.local/path/*/resource`) | matches paths starting with `/path` and ending with `/resource`									 |
| `http://*.local/path/to/resource`)             | matches paths with domains ending in `.local` and paths starting with `/path/to/resource`		 |
| `http//mockhttpclient.local/*`                 | matches everything at the domain `mockhttpclient.local`											 |
| `//mockhttpclient.local/path/to/resource`      | matches https and http																			 |
| `/path/to/resource`                            | matches all paths at `/path/to/resource` regardless of domain									 |
| `/path/to/resource?type=test`                  | matches all paths at `/path/to/resource` and have query string value `type=test`					 |
| `?type=test`                                   | matches all urls that have query string value `type=test`										 |
| `?type=test&q=*`                               | matches all urls that have query string value `type=test` and have value `q` that can be anything |


## slightly more complicated matching

If you want to do something more than the simple examples above, the `when` method can take a `Func<HttpRequestMessage, bool>`, while the `then` method can take a `Func<HttpRequestMessage, HttpResponseMessage>`. This allows you to create more complicated conditions and responses.

The library also provides some helper extension methods to do common checks on `HttpRequestMessage` and to set some common properties on `HttpResponseMessages`

```csharp
client
    .When(req => req.HasHeader("accept", "application/json"))
    .Then(req => new HttpResponseMessage()
            .WithHeader("version", "1")
            .WithJsonContent(new
            {
                FirstName = "John",
                LastName = "Doe"
            }));
```