using System;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Types;

namespace Sixgramm.FileStorage.Core.Dto.FileInfo;

public class FileInfoModuleDto
{
    public Guid SourceId { get; set; }
    public FileSource FileSource { get; set; }
    public IFormFile UploadedFile { get; set; }
}