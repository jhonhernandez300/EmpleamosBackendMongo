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