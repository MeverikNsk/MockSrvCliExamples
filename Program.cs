using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockServerClientNet;
using MockServerClientNet.Model;
using MockServerClientNet.Model.Body;
using MockSrvCliExamples;
using Refit;
using System.Net.Http.Headers;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRefitClient<ITestApiServer>(new RefitSettings
{
    ContentSerializer = new CustomContentSerializer(),
    Buffered = false,
})
    .ConfigureHttpClient(cfg =>
    {
        cfg.BaseAddress = new Uri("http://localhost:1080");
        cfg.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });

using IHost host = builder.Build();

var mockServerClient = new MockServerClient("localhost", 1080);


// set expectation
await mockServerClient.ClearAsync(HttpRequest.Request()
        .WithMethod(HttpMethod.Post)
        .WithPath("/api/v1/GetTestResponse"));

await mockServerClient.When(HttpRequest.Request()
        .WithMethod(HttpMethod.Post)
        .WithPath("/api/v1/GetTestResponse")
        .WithBody(Matchers.MatchingJsonPath("$.people[?(@.age > 35)]")),
        Times.Unlimited())
    .RespondAsync(HttpResponse.Response()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json; charset=utf-8")
        .WithBody(new TestResponse { HelloName = "test, with 35 and old age" }.SerializeToJsonString())
        .WithDelay(TimeSpan.FromSeconds(1)));

await mockServerClient.When(HttpRequest.Request()
        .WithMethod(HttpMethod.Post)
        .WithPath("/api/v1/GetTestResponse"),
        Times.Unlimited())
    .RespondAsync(HttpResponse.Response()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json; charset=utf-8")
        .WithBody(new TestResponse { HelloName = "test, default" }.SerializeToJsonString())
        .WithDelay(TimeSpan.FromSeconds(1)));

var testApiClient = new TestApiClient(host.Services.GetRequiredService<ITestApiServer>());

var request1 = new TestRequest
{
    People = new List<People>
    {
        new People {Name = "Igor", Age = 30},
    }
};
var result1 = string.Empty;

try
{
    result1 = (await testApiClient.GetResponseAsync(request1))?.SerializeToJsonString();
}
catch (ApiException ex)
{
    result1 = ex.Message;
}

var request2 = new TestRequest
{
    People = new List<People>
    {
        new People {Name = "Igor2", Age = 43}
    }
};
var result2 = string.Empty;

try
{
    result2 = (await testApiClient.GetResponseAsync(request2))?.SerializeToJsonString();
}
catch (ApiException ex)
{
    result2 = ex.Message;
}

Console.WriteLine($"Request1: {request1.SerializeToJsonString()} Response1: {result1}");
Console.WriteLine($"Request2: {request2.SerializeToJsonString()} Response2: {result2}");
Console.ReadLine();