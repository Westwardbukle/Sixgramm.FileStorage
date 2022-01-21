using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Core.Token;

namespace Sixgramm.FileStorage.Core.Services;

public class FileSaveService : IFileSaveService
{
    private readonly string _filePath;
    private readonly ITokenService _tokenService;

    public FileSaveService
    (
        IConfiguration configuration,
        ITokenService tokenService
    )
    {
        _filePath = configuration.GetValue<string>("Repo");
        _tokenService = tokenService;
    }

    public string SetFilePath(string type, Guid name)
    {
        var directoryInfo = new DirectoryInfo(_filePath);

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }


        var userDirectory = new DirectoryInfo(Path.Combine(_filePath, _tokenService.CurrentUserId().ToString()));
        if (!userDirectory.Exists)
        {
            userDirectory.Create();
        }


        return Path.Combine(_filePath,_tokenService.CurrentUserId().ToString())+"\\"+name+type;
    }
}