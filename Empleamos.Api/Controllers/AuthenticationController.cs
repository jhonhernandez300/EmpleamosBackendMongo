using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tests;

namespace Empleamos.Api.Controllers
{
    [ApiController]
    [Route("api/v1/authenticate")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly RoleManager<ApplicationRoleEntity> _roleManager;

        public AuthenticationController(UserManager<ApplicationUserEntity> userManager, RoleManager<ApplicationRoleEntity> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { message = "Invalid role request." });
            }

            try
            {
                var appRole = new ApplicationRoleEntity { Name = request.Role };
                var createRole = await _roleManager.CreateAsync(appRole);
                if (!createRole.Succeeded)
                {
                    return BadRequest(new { message = "Failed to create role." });
                }
                return Ok(new { message = "Role created successfully." });
            }
            catch (Exception ex)
            {                
                return StatusCode(500, new { message = "An error occurred while creating the role.", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponse { Message = "Invalid register request." });
            }

            try
            {
                var result = await _userManager.CreateAsync(new ApplicationUserEntity { UserName = request.Username, Email = request.Email }, request.Password);

                if (result.Succeeded)
                {
                    return Ok(new ApiResponse { Message = "Registration successful." });
                }
                else
                {
                    return BadRequest(new ApiResponse { Message = "Registration failed.", Data = result.Errors });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Message = "An error occurred.", Data = ex.Message });
            }
        }

        [HttpPost]
        [Route("RegisterAsync")]
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if (userExists != null) return new RegisterResponse { Message = "User already exists", Success = false };

                //if we get here, no user with this email..

                userExists = new ApplicationUserEntity
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    NIT = request.NIT,
                    RazonSocial = request.RazonSocial,
                    Address = request.Address,
                    City = request.City,
                    Department = request.Department,
                    ContactEmail = request.ContactEmail,
                    CreationDate = DateTime.UtcNow
                };
                var createUserResult = await _userManager.CreateAsync(userExists, request.Password);

                if (!createUserResult.Succeeded) return new RegisterResponse { Message = $"Create user failed {createUserResult?.Errors?.First()?.Description}", Success = false };
                //user is created...
                //then add user to a role...
                var addUserToRoleResult = await _userManager.AddToRoleAsync(userExists, "SUPLIER");

                if (!addUserToRoleResult.Succeeded) return new RegisterResponse { Message = $"Create user succeeded but could not add user to role {addUserToRoleResult?.Errors?.First()?.Description}", Success = false };

                //all is still well..
                return new RegisterResponse
                {
                    Success = true,
                    Message = "User registered successfully"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { Message = ex.Message, Success = false };
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Message = "Invalid model state",
                    Error = "Invalid model state",
                    Data = null
                });
            }

            var loginResponse = await LoginAsync(request);

            // Si la operación de inicio de sesión no fue exitosa
            if (!loginResponse.Success)
            {
                // Retornar un BadRequest con un mensaje apropiado
                return BadRequest(new ApiResponse
                {
                    Message = loginResponse.Message, // Asegúrate de que este mensaje es el correcto
                    Error = loginResponse.Message,
                    Data = null
                });
            }

            return Ok(loginResponse); // Retornar el token o la información del usuario
        }

        private async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                // Si no se encuentra al usuario, retornar un mensaje específico
                if (user is null)
                {
                    return new LoginResponse
                    {
                        Message = "User not found",
                        Success = false
                    };
                }

                // Verificar si la cuenta del usuario está bloqueada
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new LoginResponse
                    {
                        Message = "User account is locked", // Asegúrate de que este mensaje sea devuelto
                        Success = false
                    };
                }

                // Comprobar la contraseña
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!isPasswordValid) // Si la contraseña es incorrecta
                {
                    return new LoginResponse
                    {
                        Message = "Invalid email/password",
                        Success = false
                    };
                }

                // Si se llega aquí, las credenciales son correctas
                var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleClaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1swek3u4uo2u4a6e"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddMinutes(30);

                var token = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new LoginResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Login Successful",
                    Email = user?.Email,
                    Success = true,
                    UserId = user?.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during login." // Mensaje genérico para errores
                };
            }
        }





    }
}
