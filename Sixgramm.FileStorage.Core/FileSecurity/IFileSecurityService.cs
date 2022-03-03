using Microsoft.AspNetCore.Http;

namespace Sixgramm.FileStorage.Core.FileSecurity;

public interface IFileSecurityService
{
    public bool CheckFile(IFormFile uploadedFile, string type);
}