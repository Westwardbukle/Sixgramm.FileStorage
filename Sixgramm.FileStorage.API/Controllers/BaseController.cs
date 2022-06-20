using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.File;

namespace Sixgramm.FileStorage.API.Controllers
{
    public class BaseController : Controller
    {
        protected async Task<ActionResult> ReturnResult<T, TM>(Task<T> task) where T : ResultContainer<TM>
        {
            var result = await task;

            if (result.ErrorType.HasValue)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(),
                    ErrorType.BadRequest => BadRequest(),
                    ErrorType.Unauthorized => Unauthorized(),
                    ErrorType.UnsupportedMediaType =>StatusCode(415),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (result.Data == null)
                return NoContent();
            
            return Ok(result.Data);
        }
        
        protected async Task<ActionResult> ReturnFileResult<T, TM>(Task<T> task)
            where T : ResultContainer<TM>
            where TM : FileInfoDto
        {
            var result = await task;

            if (result.ErrorType.HasValue)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(),
                    ErrorType.BadRequest => BadRequest(),
                    ErrorType.Unauthorized => Unauthorized(),
                    ErrorType.UnsupportedMediaType => StatusCode(415),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return new PhysicalFileResult(result.Data.FilePath, "application/octet-stream")
            {
                FileDownloadName = result.Data.FileName + result.Data.FileType,
                EnableRangeProcessing = true,
            };
        }
    }
}