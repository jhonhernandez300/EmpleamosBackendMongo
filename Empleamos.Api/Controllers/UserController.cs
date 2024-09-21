using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Empleamos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
            {
                return NotFound(new { Message = "User not found" });
            }

            return NoContent();  
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            var result = await _userService.UpdateUserAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [Authorize]
        [HttpGet]        
        public async Task<List<UserEntity>> GetAll()
        {
            return await _userService.GetAll();
        }
    }
}

/* https://www.youtube.com/watch?v=vItyn5jd-k8
 * https://juandavid.site/20230729/net-core-mongo-jwt
 */