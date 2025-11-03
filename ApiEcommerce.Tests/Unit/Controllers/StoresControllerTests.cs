using Xunit;
using Moq;
using ApiEcommerce.Controllers.V1;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiEcommerce.Tests
{
    public class StoresControllerTests
    {
        private readonly Mock<IStoreRepository> _storeRepoMock;
    private readonly StoresController _controller;

        public StoresControllerTests()
        {
            _storeRepoMock = new Mock<IStoreRepository>();
            _controller = new StoresController(_storeRepoMock.Object);
        }

        [Fact]
        public void CreateStore_ReturnsCreated_WhenValid()
        {
            var dto = new CreateStoreDto { Name = "Test Store" };
            _storeRepoMock.Setup(r => r.StoreExists(dto.Name)).Returns(false);
            _storeRepoMock.Setup(r => r.CreateStore(It.IsAny<Store>())).Returns(true);
            var result = _controller.CreateStore(dto);
            Assert.IsType<CreatedAtRouteResult>(result);
        }

        [Fact]
        public void CreateStore_ReturnsBadRequest_WhenStoreExists()
        {
            var dto = new CreateStoreDto { Name = "Test Store" };
            _storeRepoMock.Setup(r => r.StoreExists(dto.Name)).Returns(true);
            var result = _controller.CreateStore(dto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateStore_ReturnsNoContent_WhenValid()
        {
            var dto = new CreateStoreDto { Name = "Updated Store" };
            _storeRepoMock.Setup(r => r.StoreExists(It.IsAny<int>())).Returns(true);
            _storeRepoMock.Setup(r => r.UpdateStore(It.IsAny<Store>())).Returns(true);
            var result = _controller.UpdateStore(1, dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateStore_ReturnsNotFound_WhenStoreNotExists()
        {
            var dto = new CreateStoreDto { Name = "Updated Store" };
            _storeRepoMock.Setup(r => r.StoreExists(It.IsAny<int>())).Returns(false);
            var result = _controller.UpdateStore(1, dto);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteStore_ReturnsNoContent_WhenValid()
        {
            var store = new Store { Id = 1, Name = "Test Store", CreationDate = System.DateTime.Now };
            _storeRepoMock.Setup(r => r.StoreExists(store.Id)).Returns(true);
            _storeRepoMock.Setup(r => r.GetStore(store.Id)).Returns(store);
            _storeRepoMock.Setup(r => r.DeleteStore(store)).Returns(true);
            var result = _controller.DeleteStore(store.Id);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteStore_ReturnsNotFound_WhenStoreNotExists()
        {
            _storeRepoMock.Setup(r => r.StoreExists(It.IsAny<int>())).Returns(false);
            var result = _controller.DeleteStore(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
