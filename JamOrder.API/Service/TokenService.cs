using System;
using System.Collections.Generic;
using JamOrder.API.Dtos;
using JamOrder.API.Encryption;
using JamOrder.API.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JamOrder.API.Service
{
    public class TokenService : ITokenService
    {
        #region Declares
        private readonly IMemoryCache _memoryCache;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly ILogger<TokenService> _logger;
        private readonly AppSettings _appSettings;
        #endregion

        public TokenService(IMemoryCache memoryCache, IEncryptionProvider encryptionProvider, IOptions<AppSettings> appSettings, ILogger<TokenService> logger)
        {
            _memoryCache = memoryCache;
            _encryptionProvider = encryptionProvider;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public TokenResponseDto GenerateToken(string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName))
                    return null;

                userName = userName.ToLower();

                var token = Guid.NewGuid().ToString();
                var tokenExpiryTime = TimeSpan.FromMinutes(_appSettings.TokenExpiry);

                _memoryCache.Set(token, userName, tokenExpiryTime);

                //track user tokens 
                _memoryCache.TryGetValue(userName, out List<string> userTokens);

                if (userTokens == null)
                    userTokens = new List<string>();

                userTokens.Add(token);

                _memoryCache.Set(userName, userTokens);

                var encryptedToken = _encryptionProvider.Encrypt(token);

                return new TokenResponseDto
                {
                    AccessToken = encryptedToken,
                    ExpireIn = tokenExpiryTime.TotalMilliseconds.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("<<<<<<<<<< an error occured while generating token", ex);
                return null;
            }
        }

        public bool DestroyToken(string userName, string encryptedToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(encryptedToken))
                    return false;

                userName = userName.ToLower();

                var decryptedToken = _encryptionProvider.Decrypt(encryptedToken);

                var token = _memoryCache.Get<string>(decryptedToken);

                if (token == null)
                    return false;

                _memoryCache.Remove(decryptedToken);

                _memoryCache.TryGetValue(userName, out List<string> userTokens);

                foreach (var userToken in userTokens)
                {
                    _memoryCache.Remove(userToken);
                }

                _memoryCache.Remove(userName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("<<<<<<<<<<< an error occured while destroying toke >>>>>>>>>>>>", ex);
                return false;
            }
        }
    }
}
