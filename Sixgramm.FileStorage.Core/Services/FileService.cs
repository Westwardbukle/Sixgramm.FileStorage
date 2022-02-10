using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Net.Http.Headers;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Common.Types;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.Dto.Upload;
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
        private readonly string[] _permittedExtensions;
        

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            ITokenService tokenService,
            IFileSaveService fileSave,
            IConfiguration configuration,
            IFileSecurityService fileSecurity
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _fileSave = fileSave;
            _permittedExtensions = configuration.GetValue<string>("Extensions").Split(",");
            _fileSecurity = fileSecurity;
        }

        public async Task<ResultContainer<FileDownloadResponseDto>> DownloadFile(FileInfoModuleDto fileInfoModuleDto)
        {
            var result = new ResultContainer<FileDownloadResponseDto>(); 

            if (fileInfoModuleDto.UploadedFile != null)
            {

                var name = Guid.NewGuid();

                var type = Path.GetExtension(fileInfoModuleDto.UploadedFile.FileName).ToLowerInvariant();

                if (_fileSecurity.CheckExtension(type))
                {
                    result.ErrorType = ErrorType.BadRequest;
                    return result;
                }


                if (_fileSecurity.CheckSignature(fileInfoModuleDto.UploadedFile, type)==false)
                {
                    result.ErrorType = ErrorType.BadRequest;
                    return result;
                }
                
                
                var path = _fileSave.SetFilePath(type, name, fileInfoModuleDto); 

                await using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await fileInfoModuleDto.UploadedFile.CopyToAsync(fileStream);
                }


                var file = new FileModel
                {
                    Name = name,
                    UserId = (Guid) _tokenService.CurrentUserId(),
                    Path = path,
                    Length = fileInfoModuleDto.UploadedFile.Length,
                    Types = type
                };
                result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(await _fileRepository.Create(file));
                return result;
            }

            result.ErrorType = ErrorType.NotFound;
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
                /*var fileUploadResponse = new FileUploadResponseDto
                {
                    Bytes = await System.IO.File.ReadAllBytesAsync(file.Path)
                };
                result = _mapper.Map<ResultContainer<FileUploadResponseDto>>(fileUploadResponse);
                return result;*/


                /*var fileUpload = new FileUploadResponseDto();
                fileUpload.PhysicalFileResult = GetFileResult(file.Path,file.Types, file.Name.ToString());
                result = _mapper.Map<ResultContainer<FileUploadResponseDto>>(fileUpload); */

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