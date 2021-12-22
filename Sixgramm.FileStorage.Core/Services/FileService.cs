using System;
using System.Threading.Tasks;
using AutoMapper;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Database.Repository.File;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
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