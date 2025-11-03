using Xunit;
using ApiEcommerce.Repository;
using ApiEcommerce.Models;
using ApiEcommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Moq;

namespace ApiEcommerce.Tests
{
    public class UserRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public UserRepositoryTests()
        {
            // 🧱 Configurar EF Core InMemory
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUserDb")
                .Options;

            _context = new ApplicationDbContext(options);

            // 🧩 Configurar dependencias simuladas
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new UserManager<ApplicationUser>(
                store.Object, null, null, null, null, null, null, null, null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new RoleManager<IdentityRole>(
                roleStore.Object, null, null, null, null
            );

            // ✅ Crear configuración real en memoria (sin Moq)
            var inMemorySettings = new Dictionary<string, string>
            {
                { "AppSettings:Secret", "FakeSecretKey123" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // 🧠 Crear repositorio real
            _repo = new UserRepository(_context, _config, _userManager, _roleManager);
        }

        [Fact]
        public void GetUser_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = _repo.GetUser("notfound");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUsers_ReturnsList()
        {
            // Arrange
            _context.ApplicationUsers.Add(new ApplicationUser
            {
                Id = "1",
                UserName = "admin",
                Email = "admin@test.com"
            });
            _context.SaveChanges();

            // Act
            var result = _repo.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void IsUniqueUser_ReturnsTrue_WhenUnique()
        {
            // Act
            var result = _repo.IsUniqueUser("unique@test.com");

            // Assert
            Assert.True(result);
        }
    }
}
