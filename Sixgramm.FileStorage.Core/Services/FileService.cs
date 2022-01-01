using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.File;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _webHostEnvironment = webHostEnvironment;
            _fileRepository = fileRepository;
            _mapper = mapper;
        }


        public async Task<ResultContainer<FileDownloadResponseDto>> DownloadFile(IFormFile uploadedFile)
        {
            var result = new ResultContainer<FileDownloadResponseDto>();

            if (uploadedFile != null)
            {
                
                string path = _webHostEnvironment.WebRootPath + "\\files\\" + uploadedFile.FileName;
                
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                FileModel file = new FileModel
                {
                    Name = uploadedFile.FileName,
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