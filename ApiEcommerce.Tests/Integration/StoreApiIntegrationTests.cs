using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiEcommerce.Models.Dtos;
using Microsoft.AspNetCore.Hosting;
using ApiEcommerce.Tests;
using Xunit;

namespace ApiEcommerce.Tests
{
    public class StoreApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public StoreApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetStores_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/v1/Stores");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateStore_ReturnsCreated()
        {
            var dto = new CreateStoreDto { Name = "Integration Store" };
            var response = await _client.PostAsJsonAsync("/api/v1/Stores", dto);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task UpdateStore_ReturnsNoContent_WhenExists()
        {
            // Primero crea una tienda
            var createDto = new CreateStoreDto { Name = "StoreToUpdate" };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/Stores", createDto);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<StoreDto>();
            // Actualiza la tienda
            var updateDto = new CreateStoreDto { Name = "StoreUpdated" };
            var updateResponse = await _client.PatchAsJsonAsync($"/api/v1/Stores/{created.Id}", updateDto);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteStore_ReturnsNoContent_WhenExists()
        {
            // Primero crea una tienda
            var createDto = new CreateStoreDto { Name = "StoreToDelete" };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/Stores", createDto);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<StoreDto>();
            // Elimina la tienda
            var deleteResponse = await _client.DeleteAsync($"/api/v1/Stores/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
