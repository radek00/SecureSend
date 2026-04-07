using System.Net;

namespace SecureSend.IntegrationTests
{
    public class UnitTest1 : IClassFixture<SecureSendWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UnitTest1(SecureSendWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            var response = await _client.GetAsync("/swagger/index.html");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
