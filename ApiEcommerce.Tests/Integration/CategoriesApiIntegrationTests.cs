using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Tests.Integration
{
    public class CategoriesApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CategoriesApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/v1/categories");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsOkOrNotFound()
        {
            var response = await _client.GetAsync("/api/v1/categories/1");
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreated()
        {
            // Nombre de 10 caracteres, sin espacios, único
            var uniqueName = $"Cat_{System.Guid.NewGuid().ToString("N").Substring(0, 10)}";
            var dto = new CreateCategoryDto { Name = uniqueName };
            var response = await _client.PostAsJsonAsync("/api/v1/categories", dto );
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }



        [Fact]
        public async Task DeleteCategory_ReturnsNoContentOrNotFound()
        {
            var response = await _client.DeleteAsync("/api/v1/categories/1");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound);
        }
        // No hay endpoint PUT para categories en la API real
    }
}
