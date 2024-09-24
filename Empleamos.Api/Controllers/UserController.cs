using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using Empleamos.Core.Services;
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
                    return NotFound(new { message = "No users found." });
                }                
                return Ok(new ApiResponse { Message = "Users retrieved successfully.", Data = users });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID." });
            }

            try
            {
                var user = await _userFacadeService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found." });
                }
                return Ok(new { message = "User retrieved successfully.", data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserEntity user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "User data is required." });
            }

            try
            {
                if (await _userFacadeService.CreateUserAsync(user))
                {
                    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { message = "User created successfully.", data = user });
                }
                return BadRequest(new { message = "Failed to create the user." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            if (id == Guid.Empty || request == null)
            {
                return BadRequest(new { message = "Invalid input." });
            }

            try
            {
                if (await _userFacadeService.UpdateUserAsync(id, request))
                {
                    return NoContent();
                }
                return BadRequest(new { message = "User ID mismatch or user not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID." });
            }

            try
            {
                if (await _userFacadeService.DeleteUserAsync(id))
                {
                    return NoContent();
                }
                return NotFound(new { message = $"User with ID {id} not found." });
            }
            catch (Exception ex)
            {                
                return StatusCode(500, new { ex.Message });
            }
        }
        
    }

}

/* https://www.youtube.com/watch?v=vItyn5jd-k8
 * https://juandavid.site/20230729/net-core-mongo-jwt
 */