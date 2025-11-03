using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using ApiEcommerce.Models;

namespace ApiEcommerce.Tests.Integration
{
    public class UsersApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UsersApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/v1/users");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ReturnsOkOrNotFound()
        {
            var response = await _client.GetAsync("/api/v1/users/1");
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreated()
        {
            var uniqueUsername = $"integrationuser_{System.Guid.NewGuid()}";
            var user = new { Username = uniqueUsername, Name = "Integration User", Password = "Test123!.", Role = "Admin" };
            var response = await _client.PostAsJsonAsync("/api/v1/users", user);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }


        // No hay endpoint PUT ni DELETE para users en la API real
    }
}
