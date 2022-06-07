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
                    ErrorType.UnsupportedMediaType => StatusCode(415),
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

            var mas = await System.IO.File.ReadAllBytesAsync(result.Data.FilePath);
            var fileType = result.Data.FileType;
            var fileName = result.Data.FileName;
            return File(mas, fileType, fileName);
        }
    }
}