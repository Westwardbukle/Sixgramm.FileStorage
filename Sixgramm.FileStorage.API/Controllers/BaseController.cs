using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;

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
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (result.Data == null)
                return NoContent();
            
            return Ok(result.Data);
        }
    }
}