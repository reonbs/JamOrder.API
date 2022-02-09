using System.Threading.Tasks;
using JamOrder.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamOrder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class AuthController : ControllerBase
    {
        #region Declares
        private readonly IUserService _userService;
        #endregion

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="loginRequestDto"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _userService.LoginAsync(loginRequestDto);

            return StatusCode(result.statusCode, result);
        }

        /// <summary>
        /// Validate a token
        /// </summary>
        /// <returns></returns>
        [HttpPost("validatetoken")]
        public IActionResult ValidateToken()
        {
            return Ok(new { message = "valid token" });
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="registerRequestDto"></param>
        /// <returns></returns>
        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var result = await _userService.Register(registerRequestDto);

            return StatusCode(result.statusCode, result);
        }

        /// <summary>
        /// Logout a user
        /// </summary>
        /// <param name="logoutRequestDto"></param>
        /// <returns></returns>
        [HttpDelete("logout")]
        public IActionResult Register([FromBody] LogoutRequestDto logoutRequestDto)
        {
            var result = _userService.LogOut(logoutRequestDto);

            return StatusCode(result.statusCode, result);
        }
    }
}