using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JamOrder.API.Dtos;
using JamOrder.API.Entities;
using JamOrder.API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace JamOrder.API.Service
{
    public class UserService : IUserService
    {
        #region Declares
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResponseService _responseService;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        public UserService(ITokenService tokenService, UserManager<ApplicationUser> userManager, IResponseService responseService, ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _responseService = responseService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiReponse<object>> Register(RegisterRequestDto registerRequestDto)
        {
            try
            {
                var user = await _userManager.CreateAsync(
                new ApplicationUser
                {
                    FirstName = registerRequestDto.FirstName,
                    LastName = registerRequestDto.LastName,
                    UserName = registerRequestDto.UserName,
                    Email = registerRequestDto.Email
                },
                registerRequestDto.Password);

                if (!user.Succeeded)
                    return _responseService.Response<object>(null, user.Errors.GetErrors(), 400);

                return _responseService.Response<object>(null, "user created successfully", 204);
            }
            catch (Exception ex)
            {
                _logger.LogError("<<<<<<<<<<<<<<< login failed >>>>>>>>>>>>>>>>>>", ex);
                return _responseService.Response<object>(null, "user registration failed", 500);
            }
        }

        public async Task<ApiReponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDto.UserName);

                if (user == null)
                    return _responseService.Response<LoginResponseDto>(null, "username or password is incorrect", 400);

                var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);

                if (!isValidPassword)
                    return _responseService.Response<LoginResponseDto>(null, "username or password is incorrect", 400);

                var generatedToken = _tokenService.GenerateToken(loginDto.UserName);

                if (generatedToken == null)
                    return _responseService.Response<LoginResponseDto>(null, "unable to retrieve token", 400);

                var loginResponse = new LoginResponseDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    AccessToken = generatedToken.AccessToken,
                    ExpiresIn = generatedToken.ExpireIn
                };

                return _responseService.Response(loginResponse, "login successful", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("<<<<<<<<<<<<<<< login failed >>>>>>>>>>>>>>>>>>", ex);
                return _responseService.Response<LoginResponseDto>(null, "login failed", 500);
            }
        }

        public ApiReponse<object> LogOut()
        {
            try
            {
                //get access token
                var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

                string accessToken = authorizationHeader.First()?.Substring("Bearer".Length)?.Trim();

                //get user claim
                var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name);

                if (user != null)
                {
                    //destroy token
                    var detroyToken = _tokenService.DestroyToken(user.Value, accessToken);

                    if (detroyToken)
                        return _responseService.Response<object>(null, "user logout successful", 200);
                }

                return _responseService.Response<object>(null, "logout failed", 400);
            }
            catch (Exception ex)
            {
                _logger.LogError("<<<<<<<< Error: logout failed >>>>>>>>>>>>>", ex);
                return _responseService.Response<object>(null, "logout failed", 500);
            }
        }
    }
}