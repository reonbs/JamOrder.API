using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using JamOrder.API.Encryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JamOrder.API
{
    public class CustomAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        #region Declares
        private readonly IMemoryCache _memoryCache;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly ILogger<string> _logger;
        #endregion

        public CustomAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock, IMemoryCache memoryCache, IEncryptionProvider encryptionProvider, ILogger<string> logger) : base(options, loggerFactory, encoder, clock)
        {
            _memoryCache = memoryCache;
            _encryptionProvider = encryptionProvider;
            _logger = logger;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers["Authorization"];

            if(string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.Fail("Unauthorized");

            if(!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unauthorized");

            string token = authorizationHeader.Substring("Bearer".Length).Trim();

            if(string.IsNullOrEmpty(token))
                return AuthenticateResult.Fail("Unauthorized");

            try
            {
                return ValidateToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"<<<<<<<<<<<<<<< authorisation failed >>>>>>>>>>>>>>>>");
                return AuthenticateResult.Fail("Unauthorized");
            }
        }

        private AuthenticateResult ValidateToken(string token)
        {
            var decryptedToken = _encryptionProvider.Decrypt(token);

            var validatedToken = _memoryCache.Get<string>(decryptedToken);

            if (string.IsNullOrWhiteSpace(validatedToken))
                return AuthenticateResult.Fail("Unauthorized");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , validatedToken)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
