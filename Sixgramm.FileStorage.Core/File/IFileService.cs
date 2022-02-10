using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Common.Types;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.Dto.Upload;

namespace Sixgramm.FileStorage.Core.File
{
    public interface IFileService
    {
        Task<ResultContainer<FileDownloadResponseDto>> DownloadFile(FileInfoModuleDto fileInfoModuleDto);
        Task<ResultContainer<PhysicalFileResult>> GetById(Guid id);
        Task<ResultContainer<FileModelResponseDto>> Delete(Guid id);
    }
}