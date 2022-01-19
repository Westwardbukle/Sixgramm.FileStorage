using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Core.Options;
using Sixgramm.FileStorage.Core.Token;

namespace Sixgramm.FileStorage.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpContext _httpContext;

        public TokenService
        (
            IHttpContextAccessor httpContextAccessor
        )
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Guid? CurrentUserId()
            => Guid.TryParse(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                ? userId : null;
    }
}