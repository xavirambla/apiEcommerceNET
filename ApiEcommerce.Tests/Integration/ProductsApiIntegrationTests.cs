using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Tests.Integration
{
    public class ProductsApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/v1/products");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetProductById_ReturnsOkOrNotFound()
        {
            var response = await _client.GetAsync("/api/v1/products/1");
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreated()
        {
            var uniqueCategoryName = $"TestCategory_{System.Guid.NewGuid()}";
            var categoryDto = new CreateCategoryDto { Name = uniqueCategoryName };
            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryDto );
            categoryResponse.EnsureSuccessStatusCode();

            var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<Category>();

            var uniqueProductName = $"IntegrationProduct_{System.Guid.NewGuid()}";
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(uniqueProductName), "Name");
            form.Add(new StringContent("Descripción de integración"), "Description");
            form.Add(new StringContent("10.5"), "Price");
            form.Add(new StringContent( createdCategory.Id.ToString() ), "CategoryId");
            form.Add(new StringContent("SKU-INT"), "SKU");
            form.Add(new StringContent("5"), "Stock");
            var response = await _client.PostAsync("/api/v1/products", form);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }


        [Fact]
        public async Task DeleteProduct_ReturnsNoContentOrNotFound()
        {
            var response = await _client.DeleteAsync("/api/v1/products/1");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound);
        }
        // El endpoint real es PUT y requiere multipart/form-data, pero para integración básica lo omitimos aquí
    }
}
