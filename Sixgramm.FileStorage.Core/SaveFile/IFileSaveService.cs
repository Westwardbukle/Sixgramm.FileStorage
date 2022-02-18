using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.FileInfo;

namespace Sixgramm.FileStorage.Core.File;

public interface IFileSaveService
{
    public void SetFilePath(string type, Guid name,Guid name720, FileInfoModuleDto fileInfoModuleDto,
        out string firstpath,out string outputPath, out string fileSource);
}