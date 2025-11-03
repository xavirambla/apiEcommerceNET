using Xunit;
using Moq;
using ApiEcommerce.Controllers;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _categoryRepoMock = new Mock<ICategoryRepository>();

            // Crea el controlador pasando los repositorios simulados
            _controller = new ProductsController(_productRepoMock.Object, _categoryRepoMock.Object);
        }

        [Fact]
        public void GetProducts_ReturnsOkWithProducts()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetProducts())
                .Returns(new List<Product> { new Product {
                         ProductId = 1,
                        Name = "prod1",
                        Category = new Category { Id = 10, Name = "cat1" } // 👈 agregado
                 } });

            // Act
            var result = _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Single(products);
        }

        [Fact]
        public void GetProduct_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetProduct(It.IsAny<int>())).Returns((Product)null);

            // Act
            var result = _controller.GetProduct(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetProduct_ReturnsOk_WhenExists()
        {
            // Arrange
            var expectedProduct = new Product {
                ProductId = 1,
                Name = "prod1",
                Category = new Category { Id = 10, Name = "cat1" } // 👈 agregado
            };
            _productRepoMock.Setup(r => r.GetProduct(1)).Returns(expectedProduct);

            // Act
            var result = _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsAssignableFrom<ProductDto>(okResult.Value);
            Assert.Equal(expectedProduct.ProductId, product.ProductId);
            Assert.Equal(expectedProduct.Name, product.Name);
        }
    }
}
