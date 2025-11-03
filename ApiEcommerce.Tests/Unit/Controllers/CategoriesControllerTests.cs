using Xunit;
using Moq;
using CategoriesControllerV1 = ApiEcommerce.Controllers.V1.CategoriesController;
using CategoriesControllerV2 = ApiEcommerce.Controllers.V2.CategoriesController;

using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiEcommerce.Tests
{
    public class CategoriesControllerV1Tests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly CategoriesControllerV1 _controller;

        public CategoriesControllerV1Tests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _controller = new CategoriesControllerV1(_categoryRepoMock.Object);
        }

        [Fact]
        public void GetCategories_ReturnsOkWithCategories()
        {
            _categoryRepoMock.Setup(r => r.GetCategories()).Returns(new List<Category> { new Category { Id = 1, Name = "cat1" } });
            var result = _controller.GetCategories();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetCategory_ReturnsNotFound_WhenNotExists()
        {
            _categoryRepoMock.Setup(r => r.GetCategory(It.IsAny<int>())).Returns((Category)null);
            var result = _controller.GetCategory(99);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetCategory_ReturnsOk_WhenExists()
        {
            _categoryRepoMock.Setup(r => r.GetCategory(It.IsAny<int>())).Returns(new Category { Id = 1, Name = "cat1" });
            var result = _controller.GetCategory(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }

    public class CategoriesControllerV2Tests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly ApiEcommerce.Controllers.V2.CategoriesController _controller;

        public CategoriesControllerV2Tests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _controller = new ApiEcommerce.Controllers.V2.CategoriesController(_categoryRepoMock.Object);
        }

        [Fact]
        public void GetCategoriesOrderById_ReturnsOkWithCategories()
        {
            _categoryRepoMock.Setup(r => r.GetCategories()).Returns(new List<Category> { new Category { Id = 1, Name = "cat1" } });
            var result = _controller.GetCategoriesOrderById();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetCategory_ReturnsNotFound_WhenNotExists()
        {
            _categoryRepoMock.Setup(r => r.GetCategory(It.IsAny<int>())).Returns((Category)null);
            var result = _controller.GetCategory(99);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetCategory_ReturnsOk_WhenExists()
        {
            _categoryRepoMock.Setup(r => r.GetCategory(It.IsAny<int>())).Returns(new Category { Id = 1, Name = "cat1" });
            var result = _controller.GetCategory(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
