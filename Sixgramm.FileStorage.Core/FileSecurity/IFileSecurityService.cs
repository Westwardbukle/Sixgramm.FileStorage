using Microsoft.AspNetCore.Http;

namespace Sixgramm.FileStorage.Core.FileSecurity;

public interface IFileSecurityService
{
    public bool FileСheck(IFormFile uploadedFile, string type);
}