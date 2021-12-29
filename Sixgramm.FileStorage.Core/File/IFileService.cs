using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.File;

namespace Sixgramm.FileStorage.Core.File
{
    public interface IFileService
    {
        Task<ResultContainer<FileModelResponseDto>> DownloadFile();
        Task<ResultContainer<FileModelResponseDto>> GetById(Guid id);
        Task<ResultContainer<FileModelResponseDto>> Delete(Guid id);
    }
}