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
        public async Task GetAllUsers_ReturnsOk_WhenUsersAreFound()
        {
            // Arrange
            var users = new List<UserEntity>
            {
                new UserEntity { Id = Guid.NewGuid(), UserName = "John Doe" },
                new UserEntity { Id = Guid.NewGuid(), UserName = "Jane Doe" }
            };
            _userFacadeServiceMock.Setup(service => service.GetAllUsersAsync())
                                  .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal("Users retrieved successfully.", responseValue.Message);
            Assert.Equal(2, ((IEnumerable<UserEntity>)responseValue.Data).Count());
        }


        
    }
}
