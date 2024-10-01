namespace MockSrvCliExamples
{
    using System.Threading.Tasks;

    internal class TestApiClient
    {
        private readonly ITestApiServer _testApiServer;
        
        public TestApiClient(ITestApiServer testApiServer)
        {
            _testApiServer = testApiServer;
        }

        public async Task<TestResponse> GetResponseAsync(TestRequest testRequest)
        {
            return await _testApiServer.GetTestResponseAsync(testRequest);
        }
    }
}
