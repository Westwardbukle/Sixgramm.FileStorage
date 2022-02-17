using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.FFMpeg;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Core.FileSecurity;
using Sixgramm.FileStorage.Core.Token;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.File;
using ContentResult = Sixgramm.FileStorage.Common.Content.ContentResult;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IFileSaveService _fileSave;
        private readonly IFileSecurityService _fileSecurity;
        private readonly IFFMpegService _ffMpegService;
        
        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            ITokenService tokenService,
            IFileSaveService fileSave,
            IFileSecurityService fileSecurity,
            IFFMpegService ffMpegService
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _fileSave = fileSave;
            _fileSecurity = fileSecurity;
            _ffMpegService = ffMpegService;
        }

        public async Task<ResultContainer<FileDownloadResponseDto>> UploadFile(FileInfoModuleDto fileInfoModuleDto)
        {
            var result = new ResultContainer<FileDownloadResponseDto>();

            if (fileInfoModuleDto.UploadedFile == null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var name = Guid.NewGuid();
            var name720 = Guid.NewGuid();

            var type = Path.GetExtension(fileInfoModuleDto.UploadedFile.FileName).ToLowerInvariant();

            if (_fileSecurity.CheckExtension(type))
            {
                result.ErrorType = ErrorType.BadRequest;
                return result;
            }

            if (!_fileSecurity.CheckSignature(fileInfoModuleDto.UploadedFile, type))
            {
                result.ErrorType = ErrorType.BadRequest;
                return result;
            }

            _fileSave.SetFilePath(type, name, name720, fileInfoModuleDto, out var firstPath, out var remakePath,out var fileSource );

            await using (var fileStream = new FileStream(firstPath, FileMode.Create))
            {
                await fileInfoModuleDto.UploadedFile.CopyToAsync(fileStream);
            }

            if (type.Contains(".mp4"))
            {
                _ffMpegService.ConvertingVideoHd(firstPath, remakePath);
            }
            
            var originalFileInfo = new FileInfo(firstPath);
            if(originalFileInfo.Exists) originalFileInfo.Delete();

            var fileInfo = new FileInfo(remakePath);
            
            var file720 = new FileModel()
            {
                Name = name720,
                UserId = _tokenService.CurrentUserId().Value,
                Path = remakePath,
                Length = fileInfo.Length,
                Types = type,
                SourceId = fileInfoModuleDto.SourceId,
                FileSource = fileSource
            };
            
            result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(await _fileRepository.Create(file720));
            return result;
        }

        public async Task<ResultContainer<PhysicalFileResult>> GetById(Guid id)
        {
            var result = new ResultContainer<PhysicalFileResult>();
            var file = await _fileRepository.GetById(id);
            if (file == null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var fileInfo = new FileInfo(file.Path);

            if (fileInfo.Exists)
            {
                var fileResult = GetFileResult(file.Path, file.Types, file.Name.ToString());
                
                result = _mapper.Map<ResultContainer<PhysicalFileResult>>(fileResult);
                return result;
            }

            result.ErrorType = ErrorType.NotFound;
            return result;
        }

        public async Task<ResultContainer<FileModelResponseDto>> Delete(Guid id)
        {
            var result = new ResultContainer<FileModelResponseDto>();
            var file = await _fileRepository.Delete(id);
            if (file == null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var fileInfo = new FileInfo(file.Path);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            result.ContentResult = ContentResult.NoContentResult;

            return result;
        }

        private static PhysicalFileResult GetFileResult(string path, string types, string name)
        {
            return new PhysicalFileResult(path, "application/octet-stream")
            {
                FileDownloadName = name + types
            };
        }
    }
}