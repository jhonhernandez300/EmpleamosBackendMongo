using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using Empleamos.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Empleamos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserFacadeService _userFacadeService;

        public UserController(IUserFacadeService userFacadeService)
        {
            _userFacadeService = userFacadeService;
        }

        [HttpGet]  
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userFacadeService.GetAllUsersAsync(); 
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userFacadeService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }      

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserEntity user)
        {
            if (await _userFacadeService.CreateUserAsync(user))
            {
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            if (await _userFacadeService.UpdateUserAsync(id, request))
            {
                return NoContent();
            }
            return BadRequest("User ID mismatch or user not found");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (await _userFacadeService.DeleteUserAsync(id))
            {
                return NoContent();
            }
            return NotFound();
        }
    }

}

/* https://www.youtube.com/watch?v=vItyn5jd-k8
 * https://juandavid.site/20230729/net-core-mongo-jwt
 */