using System;
using Sixgramm.FileStorage.Core.Dto.FileInfo;

namespace Sixgramm.FileStorage.Core.SaveFile;

public interface IFilePathService
{
    public void SetFilePath(string type, Guid name, Guid name720, FileInfoModuleDto fileInfoModuleDto,
        out string firstpath, out string outputPath, out string fileSource);
}