﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.API.Controllers;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.File;

namespace Sixgramm.FileStorage.API
{
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class TaskController : BaseController
    {
        private readonly IFileService _fileService;

        public TaskController
        (
            IFileService fileService
        )
        {
            _fileService = fileService;
        }
        
        
        
        
        
        

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Return user</response>
        /// <response code="404">If user with Id doesn't exist</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FileModelResponseDto>> GetById(Guid id)
            => await ReturnResult<ResultContainer<FileModelResponseDto>, FileModelResponseDto>
                (_fileService.GetById(id));
        
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Return file</response>
        /// <response code="404">If file doesn't exist</response>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FileModelResponseDto>> Delete(Guid id)
            => await ReturnResult<ResultContainer<FileModelResponseDto>, FileModelResponseDto>
                (_fileService.Delete(id));
    }
}