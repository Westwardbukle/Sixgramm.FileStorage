using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Base;

namespace Sixgramm.FileStorage.Core.Dto.Upload
{
    public class FileUploadResponseDto
    {
       public PhysicalFileResult PhysicalFileResult { get; set; }
    }
}