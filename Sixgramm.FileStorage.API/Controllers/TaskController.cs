using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Core.File;

namespace Sixgramm.FileStorage.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class TaskController : BaseController
    {
        private readonly IFileService _fileService;
        private const long MaxFileSize = 2L * 1024L * 1024L * 1024L;

        public TaskController
        (
            IFileService fileService
        )
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Add file
        /// </summary>
        /// <param name="fileInfoModuleDto"></param>
        /// <response code="200">Return file Id</response>
        /// <response code="415">File not supported</response>
        /// <response code="404">If the file is not found</response>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<ActionResult> UploadFile([FromForm]FileInfoModuleDto fileInfoModuleDto)
        => await ReturnResult<ResultContainer<FileDownloadResponseDto>, FileDownloadResponseDto>
                (_fileService.UploadFile(fileInfoModuleDto));

        /// <summary>
        /// Get file by Id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Return bytes file</response>
        /// <response code="404">If user with Id doesn't exist</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(Guid id)
            => await ReturnResult<ResultContainer<PhysicalFileResult>,PhysicalFileResult>
                (_fileService.GetById(id));
        
        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="id"></param>
        /// <response code="204">No content</response>
        /// <response code="404">If file doesn't exist</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FileModelResponseDto>> Delete(Guid id)
            => await ReturnResult<ResultContainer<FileModelResponseDto>, FileModelResponseDto>
                (_fileService.Delete(id));
    }
}