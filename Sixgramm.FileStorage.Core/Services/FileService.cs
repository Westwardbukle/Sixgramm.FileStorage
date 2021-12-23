using System;
using System.Threading.Tasks;
using AutoMapper;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Database;
using Sixgramm.FileStorage.Database.Repository.File;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            AppDbContext context
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _context = context;
        }

        public Task<ResultContainer<FileModelResponseDto>> Create()
        {
            throw new NotImplementedException();
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