using Xunit;
using ApiEcommerce.Repository;
using ApiEcommerce.Models;
using ApiEcommerce.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ApiEcommerce.Tests
{
    public class CategoryRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoryRepository _repo;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new CategoryRepository(_context);
        }

        [Fact]
        public void GetCategories_ReturnsList()
        {
            // Arrange
            _context.Categories.Add(new Category { Id = 1, Name = "Test" });
            _context.SaveChanges();

            // Act
            var result = _repo.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetCategory_ReturnsNull_WhenNotFound()
        {
            var result = _repo.GetCategory(99);
            Assert.Null(result);
        }

        [Fact]
        public void CategoryExists_ReturnsFalse_WhenNotFound()
        {
            var exists = _repo.CategoryExists(99);
            Assert.False(exists);
        }
    }
}
