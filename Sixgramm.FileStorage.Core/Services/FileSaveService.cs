using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

    /// <summary>
    /// Method for saving an avatar in physical space
    /// </summary>
    /// <returns>FileModel file</returns>
    public async Task<FileModel> SaveAvatar(string type, string fileSource, FileInfoModuleDto fileInfoModuleDto)
    {
        _filePath.SetAvtarPath(type, fileSource, out var firstPath, out var name);

        await using (var fileStream = new FileStream(firstPath, FileMode.Create))
        {
            await fileInfoModuleDto.UploadedFile.CopyToAsync(fileStream);
            /*using (var image = (Bitmap) Image.FromStream(fileStream))
            {
                 SaveBitmapWithQuality(image, 100, fileStream);
            }*/
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

    /// <summary>
    /// Method for saving file in physical space
    /// </summary>
    /// <returns>FileModel file</returns>
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

    /// <summary>
    /// Method for saving video file in physical space
    /// </summary>
    /// <returns>FileModel file</returns>
    public async Task<FileModel> SaveVideoFile(string type, string fileSource, string sourceId,
        FileInfoModuleDto fileInfoModuleDto)
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
    
    //Work on this feature is still ongoing

    /*private void SaveBitmapWithQuality(Bitmap bitmap, int quality, Stream outputStream)
    {
        if (quality < 0 || quality > 100)
        {
            throw new ArgumentOutOfRangeException(
                "quality", "quality must be in [0..100].");
        }

        var jpgEncoder = ImageCodecInfo
            .GetImageDecoders().Single(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
        var qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
        var encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = new EncoderParameter(qualityEncoder, quality);
        bitmap.Save(outputStream, jpgEncoder, encoderParams);
    }*/
}