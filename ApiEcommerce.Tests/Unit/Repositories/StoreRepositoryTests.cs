using Xunit;
using ApiEcommerce.Repository;
using ApiEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ApiEcommerce.Tests
{
    public class StoreRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly StoreRepository _repository;

        public StoreRepositoryTests()
        {
            // Configurar la base de datos en memoria
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "StoreTestDb_" + System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new StoreRepository(_context);
        }

        [Fact]
        public void CreateStore_ReturnsTrue_WhenValid()
        {
            // Arrange
            var store = new Store { Name = "Test Store", CreationDate = System.DateTime.UtcNow };

            // Act
            var result = _repository.CreateStore(store);

            // Assert
            Assert.True(result);
            Assert.Single(_context.Stores); // se insertó
            Assert.Equal("Test Store", _context.Stores.First().Name);
        }

        [Fact]
        public void UpdateStore_ReturnsTrue_WhenValid()
        {
            // Arrange
            var store = new Store { Name = "Old Store", CreationDate = System.DateTime.UtcNow };
            _context.Stores.Add(store);
            _context.SaveChanges();

            store.Name = "Updated Store";

            // Act
            var result = _repository.UpdateStore(store);

            // Assert
            Assert.True(result);
            var updated = _context.Stores.First();
            Assert.Equal("Updated Store", updated.Name);
        }

        [Fact]
        public void DeleteStore_ReturnsTrue_WhenValid()
        {
            // Arrange
            var store = new Store { Name = "To Delete", CreationDate = System.DateTime.UtcNow };
            _context.Stores.Add(store);
            _context.SaveChanges();

            // Act
            var result = _repository.DeleteStore(store);

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Stores);
        }
    }
}
