using ApiEcommerce.Controllers;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using FluentAssertions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiEcommerce.Tests.Unit.Controllers
{
    public class ClientsControllerTests
    {
        private readonly Mock<IClientRepository> _repoMock;
        private readonly ClientsController _controller;

        public ClientsControllerTests()
        {
            _repoMock = new Mock<IClientRepository>();
            _controller = new ClientsController(_repoMock.Object);
        }

        [Fact]
        public async Task CreateClient_Returns_Created_Client()
        {
            var dto = new CreateClientDto { Name = "Test", Phones = new(), Addresses = new() };
            var client = new Client { Id = 1, Name = "Test" };
            _repoMock.Setup(r => r.AddClientAsync(It.IsAny<Client>())).ReturnsAsync(client);

            var result = await _controller.CreateClient(dto);

            var created = result.Result as CreatedAtActionResult;
            created.Should().NotBeNull();
            var value = created!.Value as ClientDto;
            value.Should().NotBeNull();
            value!.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetClient_Returns_Client_If_Exists()
        {
            // Arrange
            var client = new Client { Id = 2, Name = "Test2" };
            _repoMock.Setup(r => r.GetClientByIdAsync(2)).ReturnsAsync(client);

            // Act
            var result = await _controller.GetClient(2);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var returnedClient = okResult.Value as ClientDto;
            returnedClient.Should().NotBeNull();
            returnedClient.Id.Should().Be(2);
            returnedClient.Name.Should().Be("Test2");
        }

        [Fact]
        public async Task GetClient_Returns_NotFound_If_Not_Exists()
        {
            _repoMock.Setup(r => r.GetClientByIdAsync(99)).ReturnsAsync((Client?)null);

            var result = await _controller.GetClient(99);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddPhone_Returns_Ok_With_PhoneDto()
        {
            var phone = new Phone { Id = 1, Number = "123", IsDefault = true };
            _repoMock.Setup(r => r.AddPhoneAsync(1, It.IsAny<Phone>())).ReturnsAsync(phone);

            var result = await _controller.AddPhone(1, new CreatePhoneDto { Number = "123", IsDefault = true });

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            var value = ok!.Value as PhoneDto;
            value.Should().NotBeNull();
            value!.Number.Should().Be("123");
        }

        [Fact]
        public async Task AddPhone_Returns_NotFound_If_Client_Not_Exists()
        {
            _repoMock.Setup(r => r.AddPhoneAsync(99, It.IsAny<Phone>())).ReturnsAsync((Phone?)null);

            var result = await _controller.AddPhone(99, new CreatePhoneDto { Number = "123", IsDefault = true });

            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
