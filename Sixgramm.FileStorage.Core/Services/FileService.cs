using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JWT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.Upload;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.File;
using ContentResult = Sixgramm.FileStorage.Common.Content.ContentResult;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly string _filePath;

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            IConfiguration configuration
            
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _filePath = configuration.GetValue<string>("Repo");
        }
        
        public async Task<ResultContainer<FileDownloadResponseDto>> DownloadFile(IFormFile uploadedFile)
        {
            
            var result = new ResultContainer<FileDownloadResponseDto>();

            if (uploadedFile != null)
            {
                var directoryInfo = new DirectoryInfo(_filePath);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                var name = new Guid();
                name=Guid.NewGuid();
                
                
               string[] type= uploadedFile.ContentType.Split("/");
                
                var path = _filePath + name +"."+type[1]; /*uploadedFile.FileName*/

                
                await using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                

                var file = new FileModel
                {
                    Name = name,
                    //UserId =,
                    Path = path,
                    Length = uploadedFile.Length,
                    Types = uploadedFile.ContentType
                };
                result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(await _fileRepository.Create(file));
                return result;
            }

            result.ErrorType = ErrorType.NotFound;
            return result;
        }
        
        public async Task<ResultContainer<FileUploadResponseDto>> GetById(Guid id)
        {
            var result = new ResultContainer<FileUploadResponseDto>();
            var file = await _fileRepository.GetById(id);
            if (file==null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var fileInfo = new FileInfo(file.Path);
            
            if (fileInfo.Exists)
            {
                var fileUploadResponse = new FileUploadResponseDto
                {
                    Bytes = await System.IO.File.ReadAllBytesAsync(file.Path)
                };
                result = _mapper.Map<ResultContainer<FileUploadResponseDto>>(fileUploadResponse);
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