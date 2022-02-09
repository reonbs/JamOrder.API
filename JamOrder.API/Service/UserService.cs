using JamOrder.API.Dtos;
using JamOrder.API.Encryption;
using JamOrder.API.Entities;
using JamOrder.API.Extensions;
using JamOrder.API.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace JamOrder.API.Service
{
    public class UserService : IUserService
    {
        #region Declares
        private readonly IMemoryCache _memoryCache;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResponseService _responseService;
        private readonly ILogger<string> _logger;
        private readonly AppSettings _appSettings;
        #endregion

        public UserService(
            IMemoryCache memoryCache,
            IEncryptionProvider encryptionProvider,
            UserManager<ApplicationUser> userManager,
            IResponseService responseService, IOptions<AppSettings> appSettings, ILogger<string> logger)
        {
            _memoryCache = memoryCache;
            _encryptionProvider = encryptionProvider;
            _userManager = userManager;
            _responseService = responseService;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<ApiReponse<object>> Register(RegisterRequestDto registerRequestDto)
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

            return new ApiReponse<object>(null, "user created successfully", 204);
        }

        public async Task<ApiReponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
                return _responseService.Response<LoginResponseDto>(null, "username or password is incorrect", 400);

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isValidPassword)
                return _responseService.Response<LoginResponseDto>(null, "username or password is incorrect", 400);

            var token = Guid.NewGuid().ToString();
            var tokenExpiryTime = TimeSpan.FromMinutes(_appSettings.TokenExpiry);

            _memoryCache.Set(token, loginDto.UserName, tokenExpiryTime);

            var encryTedToken = _encryptionProvider.Encrypt(token);

            var loginResponse = new LoginResponseDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                AccessToken = encryTedToken,
                ExpiresIn = tokenExpiryTime.TotalMilliseconds.ToString()
            };

            return _responseService.Response(loginResponse, "login successful", 200);
        }

        public ApiReponse<object> LogOut(LogoutRequestDto logoutRequestDto)
        {
            try
            {
                if (logoutRequestDto == null)
                    return _responseService.Response<object>(null, "invalid request", 400);

                var decryptedToken = _encryptionProvider.Decrypt(logoutRequestDto.AccessToken);

                var token = _memoryCache.Get<string>(decryptedToken);

                if (token == null)
                    return _responseService.Response<object>(null, "logout failed", 400);

                _memoryCache.Remove(decryptedToken);

                return _responseService.Response<object>(null, "user logout successful", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("<<<<<<<< Error: logout failed >>>>>>>>>>>>>", ex);
                return _responseService.Response<object>(null, "logout failed", 400);
            }
        }
    }
}
