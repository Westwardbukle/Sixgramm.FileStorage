using System;

namespace Sixgramm.FileStorage.Core.Token
{
    public interface ITokenService
    {
        public Guid? CurrentUserId();
    }
}