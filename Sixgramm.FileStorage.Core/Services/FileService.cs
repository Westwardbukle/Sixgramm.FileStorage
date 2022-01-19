﻿using System;
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
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.Upload;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Core.Token;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.File;
using ContentResult = Sixgramm.FileStorage.Common.Content.ContentResult;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IFileSaveService _fileSave;

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            ITokenService tokenService,
            IFileSaveService fileSave
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _fileSave = fileSave;
        }
        
        public async Task<ResultContainer<FileDownloadResponseDto>> DownloadFile(IFormFile uploadedFile)
        {
            
            var result = new ResultContainer<FileDownloadResponseDto>();
            
            if (uploadedFile != null)
            {
                var name = Guid.NewGuid();
                
                var type = Path.GetExtension(uploadedFile.FileName).ToLowerInvariant();

                var path = _fileSave.SetFilePath(type, name);
                    
                await using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                

                var file = new FileModel
                {
                    Name = name,
                    UserId = (Guid)_tokenService.CurrentUserId(),
                    Path = path,
                    Length = uploadedFile.Length,
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
            if (file==null)
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
                var physicalFile = new PhysicalFileResult(file.Path,file.Types);
                result = _mapper.Map<ResultContainer<PhysicalFileResult>>(physicalFile);
               return result;
            }

            result.ErrorType = ErrorType.NotFound;
            return result;
        }
        
        public async Task<ResultContainer<FileModelResponseDto>> Delete(Guid id)
        {
            var result = new ResultContainer<FileModelResponseDto>();
            var file = await _fileRepository.Delete(id);
            if (file==null)
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
    }
}