using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiEcommerce.Models.Dtos;
using FluentAssertions;
using Xunit;

namespace ApiEcommerce.Tests.Integration
{
    public class ClientsApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClientsApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Can_Create_And_Get_Client_With_Phones_And_Addresses()
        {
            var createDto = new CreateClientDto
            {
                Name = "Cliente Test",
                Phones = new()
                {
                    new CreatePhoneDto { Number = "111111111", IsDefault = true },
                    new CreatePhoneDto { Number = "222222222", IsDefault = false }
                },
                Addresses = new()
                {
                    new CreateAddressDto { Street = "Calle 1", City = "Ciudad", State = "Estado", ZipCode = "12345", IsDefault = true },
                    new CreateAddressDto { Street = "Calle 2", City = "Ciudad", State = "Estado", ZipCode = "54321", IsDefault = false }
                }
            };

            var postResponse = await _client.PostAsJsonAsync("/api/v1/clients", createDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await postResponse.Content.ReadFromJsonAsync<ClientDto>();
            created.Should().NotBeNull();
            created!.Phones.Should().HaveCount(2);
            created.Phones.Should().ContainSingle(p => p.IsDefault);
            created.Addresses.Should().HaveCount(2);
            created.Addresses.Should().ContainSingle(a => a.IsDefault);

            var getResponse = await _client.GetAsync($"/api/v1/clients/{created.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetched = await getResponse.Content.ReadFromJsonAsync<ClientDto>();
            fetched.Should().NotBeNull();
            fetched!.Phones.Should().HaveCount(2);
            fetched.Phones.Should().ContainSingle(p => p.IsDefault);
            fetched.Addresses.Should().HaveCount(2);
            fetched.Addresses.Should().ContainSingle(a => a.IsDefault);
        }

        [Fact]
        public async Task Can_Add_And_Set_Default_Phone()
        {
            var createDto = new CreateClientDto { Name = "Cliente Phone", Phones = new(), Addresses = new() };
            var postResponse = await _client.PostAsJsonAsync("/api/v1/clients", createDto);
            var client = await postResponse.Content.ReadFromJsonAsync<ClientDto>();

            var addPhone = new CreatePhoneDto { Number = "333333333", IsDefault = true };
            var phoneResponse = await _client.PostAsJsonAsync($"/api/v1/clients/{client!.Id}/phones", addPhone);
            phoneResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var phone = await phoneResponse.Content.ReadFromJsonAsync<PhoneDto>();
            phone!.IsDefault.Should().BeTrue();

            var addPhone2 = new CreatePhoneDto { Number = "444444444", IsDefault = false };
            var phoneResponse2 = await _client.PostAsJsonAsync($"/api/v1/clients/{client.Id}/phones", addPhone2);
            phoneResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
            var phone2 = await phoneResponse2.Content.ReadFromJsonAsync<PhoneDto>();
            phone2!.IsDefault.Should().BeFalse();

            // Set second as default
            var setDefault = await _client.PutAsync($"/api/v1/clients/{client.Id}/phones/{phone2.Id}/default", null);
            setDefault.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getClient = await _client.GetAsync($"/api/v1/clients/{client.Id}");
            var updated = await getClient.Content.ReadFromJsonAsync<ClientDto>();
            updated!.Phones.Should().ContainSingle(p => p.IsDefault && p.Id == phone2.Id);
        }

        [Fact]
        public async Task Can_Add_And_Set_Default_Address()
        {
            var createDto = new CreateClientDto { Name = "Cliente Address", Phones = new(), Addresses = new() };
            var postResponse = await _client.PostAsJsonAsync("/api/v1/clients", createDto);
            var client = await postResponse.Content.ReadFromJsonAsync<ClientDto>();

            var addAddress = new CreateAddressDto { Street = "Calle 3", City = "Ciudad", State = "Estado", ZipCode = "11111", IsDefault = true };
            var addressResponse = await _client.PostAsJsonAsync($"/api/v1/clients/{client!.Id}/addresses", addAddress);
            addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var address = await addressResponse.Content.ReadFromJsonAsync<AddressDto>();
            address!.IsDefault.Should().BeTrue();

            var addAddress2 = new CreateAddressDto { Street = "Calle 4", City = "Ciudad", State = "Estado", ZipCode = "22222", IsDefault = false };
            var addressResponse2 = await _client.PostAsJsonAsync($"/api/v1/clients/{client.Id}/addresses", addAddress2);
            addressResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
            var address2 = await addressResponse2.Content.ReadFromJsonAsync<AddressDto>();
            address2!.IsDefault.Should().BeFalse();

            // Set second as default
            var setDefault = await _client.PutAsync($"/api/v1/clients/{client.Id}/addresses/{address2.Id}/default", null);
            setDefault.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getClient = await _client.GetAsync($"/api/v1/clients/{client.Id}");
            var updated = await getClient.Content.ReadFromJsonAsync<ClientDto>();
            updated!.Addresses.Should().ContainSingle(a => a.IsDefault && a.Id == address2.Id);
        }
    }
}
