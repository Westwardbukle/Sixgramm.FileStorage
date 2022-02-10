using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.FileInfo;

namespace Sixgramm.FileStorage.Core.File;

public interface IFileSaveService
{
    string SetFilePath(string type, Guid name, FileInfoModuleDto fileInfoModuleDto);
}