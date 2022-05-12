using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Common.Types;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Core.FileSave;
using Sixgramm.FileStorage.Core.FileSecurity;
using Sixgramm.FileStorage.Database.Repository.File;
using ContentResult = Sixgramm.FileStorage.Common.Content.ContentResult;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly IFileSecurityService _fileSecurity;
        private readonly IFileSaveService _fileSave;

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            IFileSecurityService fileSecurity,
            IFileSaveService fileSave
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _fileSecurity = fileSecurity;
            _fileSave = fileSave;
        }

        public async Task<ResultContainer<FileDownloadResponseDto>> UploadFile(FileInfoModuleDto fileInfoModuleDto)
        {
            var result = new ResultContainer<FileDownloadResponseDto>();

            if (fileInfoModuleDto.UploadedFile == null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var type = Path.GetExtension(fileInfoModuleDto.UploadedFile.FileName).ToLowerInvariant();

            var fileSource = fileInfoModuleDto.FileSource switch
            {
                FileSource.Post => "Post",
                FileSource.Message => "Message",
                FileSource.Comment => "Comment",
                _ => "Avatar"
            };

            if (_fileSecurity.CheckFile(fileInfoModuleDto.UploadedFile, type))
            {
                result.ErrorType = ErrorType.UnsupportedMediaType;
                return result;
            }
            
            if (fileInfoModuleDto.FileSource == FileSource.Avatar)
            {
                if (!(type.Contains(".jpg") || type.Contains(".jpeg") || type.Contains(".png")))
                {
                    result.ErrorType = ErrorType.UnsupportedMediaType;
                    return result;
                }

                var avatar = await _fileSave.SaveAvatar(type, fileSource, fileInfoModuleDto);
                result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(
                    await _fileRepository.Create(avatar));
                return result;
            }

            if (type.Contains(".mp4"))
            {
                var videoFile = await _fileSave.SaveVideoFile(type, fileSource, fileInfoModuleDto.SourceId.ToString(),
                    fileInfoModuleDto);
                result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(await _fileRepository.Create(videoFile));
                return result;
            }

            var fileModel = await _fileSave.SaveFile(type, fileSource, fileInfoModuleDto);
            result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(await _fileRepository.Create(fileModel));
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

            if (fileInfo.Exists) fileInfo.Delete();

            result.ContentResult = ContentResult.NoContentResult;

            return result;
        }

        private static PhysicalFileResult GetFileResult(string path, string types, string name)
        {
            return new PhysicalFileResult(path, "application/octet-stream")
            {
                FileDownloadName = name + types,
                EnableRangeProcessing = true,
                FileName = name,
            };
        }
    }
}