using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Database;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.File;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public FileService
        (
            IWebHostEnvironment environment,
            IFileRepository fileRepository,
            IMapper mapper
        )
        {
            _environment = environment;
            _fileRepository = fileRepository;
            _mapper = mapper;
        }
        
        /*public async Task<ResultContainer<FileModelResponseDto>> DownloadFile(IFormFile uploadedFile)
        {
            var result = new ResultContainer<FileModelResponseDto>();
            var file = await _fileRepository.DownloadFile(new FileModel());
            if (uploadedFile!=null) 
            {
                string path = "/Files/" + uploadedFile.FileName;
                using (var fileStream = new FileStream
                    ( _environment.WebRootPath+"\\files\\" + uploadedFile.FileName, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                
                file ={  = uploadedFile.Name, Path = path, Typ = filetype, FileWeight = lenght };
                
                result = _mapper.Map<ResultContainer<FileModelResponseDto>>();
                return result;
            }
            result.ErrorType = ErrorType.BadRequest;
            return result;
        }*/

        public Task<ResultContainer<FileModelResponseDto>> DownloadFile()
        {
            var result = new ResultContainer<FileModelResponseDto>();
            var file = await _fileRepository.DownloadFile();
            if (file==null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            result = _mapper.Map<ResultContainer<FileModelResponseDto>>(file);
            return result;
        }

        public async Task<ResultContainer<FileModelResponseDto>> GetById(Guid id)
        {
            var result = new ResultContainer<FileModelResponseDto>();
            var file = await _fileRepository.GetById(id);
            if (file==null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            result = _mapper.Map<ResultContainer<FileModelResponseDto>>(file);
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
            result = _mapper.Map<ResultContainer<FileModelResponseDto>>(file);
            return result;
        }
        
    }
}