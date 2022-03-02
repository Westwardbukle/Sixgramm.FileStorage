using Microsoft.AspNetCore.Http;

namespace Sixgramm.FileStorage.Core.FileSecurity;

public interface IFileSecurityService
{
    /*public bool CheckExtension(string type);
    public bool CheckSignature(IFormFile uploadedFile, string type);*/
    public bool CheckFile(IFormFile uploadedFile, string type);
}