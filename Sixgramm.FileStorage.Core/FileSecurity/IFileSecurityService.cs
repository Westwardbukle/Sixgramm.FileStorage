using Microsoft.AspNetCore.Http;

namespace Sixgramm.FileStorage.Core.FileSecurity;

public interface IFileSecurityService
{
    public bool CheckExtension(IFormFile uploadedFile);
}