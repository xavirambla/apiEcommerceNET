using Xunit;
using Moq;
using ApiEcommerce.Controllers;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiEcommerce.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _controller = new UsersController(_userRepoMock.Object);
        }

        [Fact]
        public void GetUsers_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<UserDto> { new UserDto { Id = "1", Username = "test" } };
            _userRepoMock.Setup(r => r.GetUsers()).Returns(new List<ApiEcommerce.Models.ApplicationUser>());
            // Act
            var result = _controller.GetUsers();
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetUser(It.IsAny<string>())).Returns((ApiEcommerce.Models.ApplicationUser)null);
            // Act
            var result = _controller.GetUser("notfound");
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetUser(It.IsAny<string>())).Returns(new ApiEcommerce.Models.ApplicationUser { Id = "1", UserName = "test" });
            // Act
            var result = _controller.GetUser("1");
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
