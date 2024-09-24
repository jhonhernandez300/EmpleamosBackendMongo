using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tests;

namespace Empleamos.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserFacadeService _userFacadeService;

        public UserController(IUserFacadeService userFacadeService)
        {
            _userFacadeService = userFacadeService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userFacadeService.GetAllUsersAsync();
                if (users == null || !users.Any())
                {
                    return NotFound(new ApiResponse { Message = "No users found.", Data = null }); // cambio
                }
                return Ok(new ApiResponse { Message = "Users retrieved successfully.", Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Message = "An error occurred." });
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse { Message = "Invalid user ID.", Data = null }); // cambio
            }

            try
            {
                var user = await _userFacadeService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse { Message = $"User with ID {id} not found.", Data = null }); // cambio
                }
                return Ok(new ApiResponse { Message = "User retrieved successfully.", Data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Message = "An error occurred.", Error = ex.Message }); // cambio
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserEntity user)
        {
            if (user == null)
            {
                return BadRequest(new ApiResponse { Message = "User data is required.", Data = null }); // cambio
            }

            try
            {
                if (await _userFacadeService.CreateUserAsync(user))
                {
                    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new ApiResponse { Message = "User created successfully.", Data = user });
                }
                return BadRequest(new ApiResponse { Message = "Failed to create the user.", Data = null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Message = "An error occurred.", Error = ex.Message });
            }
        }

        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            if (id == Guid.Empty || request == null)
            {
                return BadRequest(new ApiResponse { Message = "Invalid input.", Data = null }); // cambio
            }

            try
            {
                if (await _userFacadeService.UpdateUserAsync(id, request))
                {
                    return Ok(new ApiResponse { Message = "User updated successfully.", Data = request }); // cambio
                }
                return BadRequest(new ApiResponse { Message = "User ID mismatch or user not found.", Data = null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Message = "An error occurred.", Error = ex.Message });
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse { Message = "Invalid user ID.", Data = null }); // cambio
            }

            try
            {
                if (await _userFacadeService.DeleteUserAsync(id))
                {
                    return Ok(new ApiResponse { Message = "User deleted successfully.", Data = null }); // cambio
                }
                return NotFound(new ApiResponse { Message = $"User with ID {id} not found.", Data = null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Message = "An error occurred.", Error = ex.Message });
            }
        }
    }
}
