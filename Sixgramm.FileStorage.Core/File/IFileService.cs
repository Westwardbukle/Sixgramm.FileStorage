using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.FileInfo;


namespace Sixgramm.FileStorage.Core.File
{
    public interface IFileService
    {
        Task<ResultContainer<FileDownloadResponseDto>> UploadFile(FileInfoModuleDto fileInfoModuleDto);
        Task<ResultContainer<FileInfoDto>> GetById(Guid id);
        Task<ResultContainer<FileModelResponseDto>> Delete(Guid id);
    }
}