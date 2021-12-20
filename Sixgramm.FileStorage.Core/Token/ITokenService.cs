using Sixgramm.FileStorage.Core.Dto.File;

namespace Sixgramm.FileStorage.Core.Token
{
    public interface ITokenService
    {
        TokenModel CreateToken(FileModelDto file);
    }
}