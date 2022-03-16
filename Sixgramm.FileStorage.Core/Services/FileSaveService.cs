using System.IO;
using System.Threading.Tasks;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.FFMpeg;
using Sixgramm.FileStorage.Core.FileSave;
using Sixgramm.FileStorage.Core.SaveFile;
using Sixgramm.FileStorage.Core.Token;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Core.Services;

public class FileSaveService : IFileSaveService
{
    private readonly IFilePathService _filePath;
    private readonly ITokenService _tokenService;
    private readonly IFFMpegService _ffMpegService;


    public FileSaveService
    (
        IFilePathService filePath,
        ITokenService tokenService,
        IFFMpegService ffMpegService
    )
    {
        _filePath = filePath;
        _tokenService = tokenService;
        _ffMpegService = ffMpegService;
    }

    public async Task<FileModel> SaveAvatar(string type, string fileSource, FileInfoModuleDto fileInfoModuleDto)
    {
        _filePath.SetAvtarPath(type, fileSource, out var firstPath, out var name);
        await using (var fileStream = new FileStream(firstPath, FileMode.Create))
        {
            await fileInfoModuleDto.UploadedFile.CopyToAsync(fileStream);
        }

        var fileInfo = new FileInfo(firstPath);
        
        var file = new FileModel()
        {
            Name = name,
            UserId = _tokenService.CurrentUserId().Value,
            Path = firstPath,
            Length = fileInfo.Length,
            Types = type,
            SourceId = fileInfoModuleDto.SourceId,
            FileSource = fileSource
        };
        return file;
    }

    public async Task<FileModel> SaveFile(string type, string fileSource, FileInfoModuleDto fileInfoModuleDto)
    {
        _filePath.SetFilePath(type, fileSource, fileInfoModuleDto.SourceId, out var firstPath, out var name);
        await using (var fileStream = new FileStream(firstPath, FileMode.Create))
        {
            await fileInfoModuleDto.UploadedFile.CopyToAsync(fileStream);
        }

        var fileInfo = new FileInfo(firstPath);
        
        var file = new FileModel()
        {
            Name = name,
            UserId = _tokenService.CurrentUserId().Value,
            Path = firstPath,
            Length = fileInfo.Length,
            Types = type,
            SourceId = fileInfoModuleDto.SourceId,
            FileSource = fileSource
        };
        return file;
    }

    public async Task<FileModel> SaveVideoFile(string type, string fileSource, string sourceId, FileInfoModuleDto fileInfoModuleDto)
    {
        _filePath.SetVideoPath(type, fileSource, sourceId, out var firstPath, out var outputPath, out var name,
            out var videoName720);
        
        await using (var fileStream = new FileStream(firstPath, FileMode.Create))
        {
            await fileInfoModuleDto.UploadedFile.CopyToAsync(fileStream);
        }
        await _ffMpegService.ConvertingVideoHd(firstPath, outputPath);
        var fileVideo = new FileInfo(firstPath);
        fileVideo.Delete();
        
        var fileMp4Info = new FileInfo(outputPath);
        
        var file = new FileModel()
        {
            Name = videoName720,
            UserId = _tokenService.CurrentUserId().Value,
            Path = outputPath,
            Length = fileMp4Info.Length,
            Types = type,
            SourceId = fileInfoModuleDto.SourceId,
            FileSource = fileSource
        };
        return file;
    }
}