namespace MockSrvCliExamples
{
    using Refit;
    using System.Threading.Tasks;


    internal interface ITestApiServer
    {
        [Post("/api/v1/GetTestResponse")]
        public Task<TestResponse> GetTestResponseAsync(TestRequest testRequest);
    }
}
