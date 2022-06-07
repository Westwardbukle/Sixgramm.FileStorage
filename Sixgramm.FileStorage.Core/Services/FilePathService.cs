using System;
using System.IO;
using Microsoft.Extensions.Configuration;
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
    
    /// <summary>
    /// Builds a path to save the video file
    /// </summary>
    public void SetVideoPath(string type, string fileSource, string sourceId,
        out string firstPath, out string outputPath, out Guid name, out Guid videoName720)
    {
        name = Guid.NewGuid();
        videoName720 = Guid.NewGuid();
        
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
            fileSource, sourceId));
        if (!sourceDirectory.Exists)
        {
            sourceDirectory.Create();
        }

        firstPath = sourceDirectory.FullName + "\\" + name + type;

        outputPath = sourceDirectory.FullName + "\\" + videoName720 + type;
    }
    
    /// <summary>
    /// Builds a path to save the avatar
    /// </summary>
    public void SetAvtarPath(string type, string fileSource, out string firstpath, out Guid name)
    {
        name = Guid.NewGuid();

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
            fileSource));
        if (!sourceDirectory.Exists)
        {
            sourceDirectory.Create();
        }

        firstpath = sourceDirectory.FullName + "\\" + name + type;
    }
    
    /// <summary>
    /// Builds a path to save the file
    /// </summary>
    public void SetFilePath(string type, string fileSource, Guid sourceId , out string firstPath, out Guid name)
    {
        name = Guid.NewGuid();

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
            fileSource, sourceId.ToString()));
        if (!sourceDirectory.Exists)
        {
            sourceDirectory.Create();
        }

        firstPath = sourceDirectory.FullName + "\\" + name + type;
    }
}