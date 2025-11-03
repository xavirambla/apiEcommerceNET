using Xunit;
using ApiEcommerce.Repository;
using ApiEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ApiEcommerce.Tests
{
    public class ProductRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _repo;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb_ProductRepo")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new ProductRepository(_context);
        }

        [Fact]
        public void GetProducts_ReturnsList()
        {
            // Arrange
            _context.Products.Add(new Product { Name = "Prod1", Category = new Category { Name = "Cat1" } });
            _context.SaveChanges();

            // Act
            var result = _repo.GetProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetProduct_ReturnsNull_WhenNotFound()
        {
            var result = _repo.GetProduct(999);
            Assert.Null(result);
        }

        [Fact]
        public void ProductExists_ReturnsFalse_WhenNotFound()
        {
            var exists = _repo.ProductExists(999);
            Assert.False(exists);
        }
    }
}
