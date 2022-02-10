﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sixgramm.FileStorage.Common.Types;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
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

    public List<string> SetFilePath(string type, Guid name, FileInfoModuleDto fileInfoModuleDto)
    {
        var info = new List<string>();
        
        var fileSource = fileInfoModuleDto.FileSource switch
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
        
        var path = sourceDirectory.FullName + "\\" + name + type;
        info.Add(path);
        info.Add(fileSource);
        
        return info;

        //return Path.Combine(_filePath,_tokenService.CurrentUserId().ToString(), fileSource, fileInfoModuleDto.PostId.ToString())+"\\"+name+type;
    }
}