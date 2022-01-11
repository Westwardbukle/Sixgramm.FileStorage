using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Sixgramm.FileStorage.Core.Options;
using Sixgramm.FileStorage.Core.Token;

namespace Sixgramm.FileStorage.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;

        public TokenService
        (
            AppOptions appOptions
        )
        {
            _secretKey = appOptions.SecretKey;
        }

        /*public TokenModel WriteToken()
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            
        }*/
    }
}