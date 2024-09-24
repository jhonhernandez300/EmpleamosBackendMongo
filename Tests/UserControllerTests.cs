using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Empleamos.Api.Controllers;
using Empleamos.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Empleamos.Core.DTOs;
using Empleamos.Core.Interfaces;

namespace Tests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IUserFacadeService> _userFacadeServiceMock;

        public UserControllerTests()
        {
            _userFacadeServiceMock = new Mock<IUserFacadeService>();
            _controller = new UserController(_userFacadeServiceMock.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsNotFound_WhenNoUsersAreFound()
        {
            // Arrange
            _userFacadeServiceMock.Setup(service => service.GetAllUsersAsync())
                                  .ReturnsAsync(new List<UserEntity>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("No users found.", response.Message);
            Assert.Null(response.Data); // No debería haber datos cuando no se encuentran usuarios
        }

        [Fact]
        public async Task GetAllUsers_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "An error occurred."; // Asegúrate de que este mensaje coincide con el controlador
            _userFacadeServiceMock.Setup(service => service.GetAllUsersAsync())
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            var response = Assert.IsType<ApiResponse>(serverErrorResult.Value);
            Assert.Equal("An error occurred.", response.Message); // Verifica el nuevo mensaje
            Assert.Null(response.Data); // Asegúrate de que el Data esté vacío
        }

        [Fact]
        public async Task GetAllUsers_ReturnsNotFound_WhenUserListIsEmpty()
        {
            // Arrange
            var emptyUsers = new List<UserEntity>();
            _userFacadeServiceMock.Setup(service => service.GetAllUsersAsync())
                                    .ReturnsAsync(emptyUsers);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.Equal("No users found.", response.Message);
            Assert.Null(response.Data); // Sin usuarios, el campo Data debe ser null
        }

        [Fact]
        public async Task GetAllUsers_ReturnsCorrectUserCount()
        {
            // Arrange
            var users = new List<UserEntity>
            {
                new UserEntity { Id = Guid.NewGuid(), UserName = "John Doe" },
                new UserEntity { Id = Guid.NewGuid(), UserName = "Jane Doe" },
                new UserEntity { Id = Guid.NewGuid(), UserName = "Alice Smith" }
            };
            _userFacadeServiceMock.Setup(service => service.GetAllUsersAsync())
                                  .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal("Users retrieved successfully.", response.Message);
            Assert.Equal(3, ((IEnumerable<UserEntity>)response.Data).Count());
        }
    } // Ensure this closing brace is here
} // Ensure this closing brace is here
