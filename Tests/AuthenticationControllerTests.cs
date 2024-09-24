using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Empleamos.Api.Controllers;  
using Empleamos.Core.Entities;      
using Microsoft.AspNetCore.Identity;
using System;
using Empleamos.Core.DTOs;

namespace Tests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<UserManager<ApplicationUserEntity>> _mockUserManager;
        private readonly Mock<RoleManager<ApplicationRoleEntity>> _mockRoleManager;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {            
            _mockUserManager = MockUserManager<ApplicationUserEntity>();
            _mockRoleManager = MockRoleManager<ApplicationRoleEntity>();
                        
            _controller = new AuthenticationController(_mockUserManager.Object, _mockRoleManager.Object);
        }
        
        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
        
        private Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
        {
            var store = new Mock<IRoleStore<TRole>>();
            var roleValidators = new List<IRoleValidator<TRole>> { new RoleValidator<TRole>() };
            return new Mock<RoleManager<TRole>>(store.Object, roleValidators, null, null, null);
        }

        [Fact]
        public async Task CreateRole_ReturnsBadRequest_WhenRequestIsNull()
        {
            
            var result = await _controller.CreateRole(null);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            
            Assert.Contains("Invalid role request.", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CreateRole_ReturnsBadRequest_WhenRoleIsEmpty()
        {
            
            var request = new CreateRoleRequest { Role = string.Empty };

            
            var result = await _controller.CreateRole(request);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            
            var badRequestValue = badRequestResult.Value;
            var messageProperty = badRequestValue.GetType().GetProperty("message").GetValue(badRequestValue, null);

            Assert.Equal("Invalid role request.", messageProperty);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRequestIsNull()
        {
            var result = await _controller.Register(null);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Invalid register request.", response.Message);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            var request = new RegisterRequest
            {
                Username = "newUser",
                Password = "Password123!",
                Email = "newuser@example.com"
            };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUserEntity>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiResponse>(okResult.Value);

            Assert.Equal("Registration successful.", value.Message);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenRegistrationIsSuccessful()
        {
            var request = new RegisterRequest
            {
                FullName = "New User",
                Email = "newuser@example.com",
                Password = "Password123!",
                NIT = "123456789",
                RazonSocial = "Test Company",
                Address = "Test Address",
                City = "Test City",
                Department = "Test Department",
                ContactEmail = "contact@example.com"
            };

            
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((ApplicationUserEntity)null);

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUserEntity>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUserEntity>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.RegisterAsync(request);

            Assert.True(result.Success);
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailure_WhenUserAlreadyExists()
        {
            var request = new RegisterRequest
            {
                FullName = "New User",
                Email = "existinguser@example.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(new ApplicationUserEntity());

            var result = await _controller.RegisterAsync(request);

            Assert.False(result.Success);
            Assert.Equal("User already exists", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailure_WhenCreateUserFails()
        {
            var request = new RegisterRequest
            {
                FullName = "New User",
                Email = "newuser@example.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((ApplicationUserEntity)null);

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUserEntity>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to create user" }));

            var result = await _controller.RegisterAsync(request);

            Assert.False(result.Success);
            Assert.Equal("Create user failed Failed to create user", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailure_WhenAddUserToRoleFails()
        {
            var request = new RegisterRequest
            {
                FullName = "New User",
                Email = "newuser@example.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((ApplicationUserEntity)null);

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUserEntity>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUserEntity>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to add user to role" }));

            var result = await _controller.RegisterAsync(request);

            Assert.False(result.Success);
            Assert.Equal("Create user succeeded but could not add user to role Failed to add user to role", result.Message);
        }

             
        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUserDoesNotExist()
        {            
            var request = new LoginRequest
            {
                Email = "nonexistentuser@example.com",
                Password = "AnyPassword123!"
            };

         
            _mockUserManager.Setup(um => um.FindByEmailAsync(request.Email))
                            .ReturnsAsync((ApplicationUserEntity)null);

           
            var result = await _controller.Login(request);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            
            var value = Assert.IsType<ApiResponse>(badRequestResult.Value);

            
            Assert.Equal("User not found", value.Message);

            
            Assert.Equal("User not found", value.Error);

            
            Assert.Null(value.Data);
        }



        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsIncorrect()
        {
            var request = new LoginRequest
            {
                Email = "validuser@example.com",
                Password = "IncorrectPassword123!"
            };

            var user = new ApplicationUserEntity { UserName = request.Email };
            _mockUserManager.Setup(um => um.FindByEmailAsync(request.Email))
                            .ReturnsAsync(user);

            
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, request.Password))
                            .ReturnsAsync(false);

            var result = await _controller.Login(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResponse>(badRequestResult.Value);

            
            Assert.Equal("Invalid email/password", value.Message);
        }



        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUserIsLockedOut()
        {
           
            var request = new LoginRequest
            {
                Email = "lockedoutuser@example.com",
                Password = "ValidPassword123!"
            };

           
            var user = new ApplicationUserEntity
            {
                UserName = request.Email,
                LockoutEnabled = true
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(request.Email))
                            .ReturnsAsync(user);

           
            _mockUserManager.Setup(um => um.IsLockedOutAsync(user))
                            .ReturnsAsync(true);

           
            var result = await _controller.Login(request);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            
            var value = Assert.IsType<ApiResponse>(badRequestResult.Value);

            
            Assert.Equal("User account is locked", value.Message);

           
            Assert.Equal("User account is locked", value.Error); 

            
            Assert.Null(value.Data);
        }

    }
}