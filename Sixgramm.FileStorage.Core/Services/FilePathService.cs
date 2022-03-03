using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sixgramm.FileStorage.Common.Types;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Core.SaveFile;
using Sixgramm.FileStorage.Core.Token;

namespace Sixgramm.FileStorage.Core.Services;

public class FilePathService : IFilePathService
{
    private readonly string _filePath;
    private readonly ITokenService _tokenService;

    public FilePathService
    (
        IConfiguration configuration,
        ITokenService tokenService
    )
    {
        _filePath = configuration.GetValue<string>("Repo");
        _tokenService = tokenService;
    }

    public void SetFilePath(string type, Guid name, Guid name720, FileInfoModuleDto fileInfoModuleDto,
        out string firstpath, out string outputPath, out string fileSource)
    {
        fileSource = fileInfoModuleDto.FileSource switch
        {
            FileSource.Post => "Post",
            FileSource.Message => "Message",
            FileSource.Comment => "Comment",
            _ => "Avatar"
        };

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

        var sourceDirectory = new DirectoryInfo(Path.Combine(_filePath, _tokenService.CurrentUserId().ToString(),
            fileSource, fileInfoModuleDto.SourceId.ToString()));
        if (!sourceDirectory.Exists)
        {
            sourceDirectory.Create();
        }

        firstpath = sourceDirectory.FullName + "\\" + name + type;

        outputPath = sourceDirectory.FullName + "\\" + name720 + type;
    }
}