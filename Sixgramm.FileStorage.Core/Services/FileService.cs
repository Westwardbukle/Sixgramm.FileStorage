using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Net.Http.Headers;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.Upload;
using Sixgramm.FileStorage.Core.File;
using Sixgramm.FileStorage.Core.Token;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.File;
using ContentResult = Sixgramm.FileStorage.Common.Content.ContentResult;

namespace Sixgramm.FileStorage.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IFileSaveService _fileSave;
        
        private static Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            { ".doc", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                }
            },
            { ".docx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                }
            },
            { ".xls", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1  }
                }
            },
            { ".xlsx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                }
            },
            { ".mp3", new List<byte[]>
                {
                    new byte[] { 0x49, 0x44, 0x33 }
                }
            },
            { ".gif", new List<byte[]>
                {
                    new byte[] { 0x47, 0x49, 0x46, 0x38 }
                }
            },
            { ".png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            }
        };

        public FileService
        (
            IFileRepository fileRepository,
            IMapper mapper,
            ITokenService tokenService,
            IFileSaveService fileSave
        )
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _fileSave = fileSave;
        }

        public async Task<ResultContainer<FileDownloadResponseDto>> DownloadFile(IFormFile uploadedFile)
        {
            var result = new ResultContainer<FileDownloadResponseDto>();

            if (uploadedFile != null)
            {

                var name = Guid.NewGuid();

                var type = Path.GetExtension(uploadedFile.FileName).ToLowerInvariant();

                var path = _fileSave.SetFilePath(type, name);

                await using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                     /*using (var reader = new BinaryReader(fileStream))
                     {
                         var key = Path.GetExtension(uploadedFile.FileName);
                         if(_fileSignature.ContainsKey(key))
                         {
                             var signatures = _fileSignature[key];
                             var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                             if (signatures.Any(signature =>
                                     headerBytes.Take(signature.Length).SequenceEqual(signature))==false)
                             {
                                 result.ErrorType = ErrorType.UnsupportedMediaType;
                             }
                         }
                         else
                         {
                             result.ErrorType = ErrorType.UnsupportedMediaType;
                         }
                     }*/
                }


                var file = new FileModel
                {
                    Name = name,
                    UserId = (Guid) _tokenService.CurrentUserId(),
                    Path = path,
                    Length = uploadedFile.Length,
                    Types = type
                };
                result = _mapper.Map<ResultContainer<FileDownloadResponseDto>>(await _fileRepository.Create(file));
                return result;
            }

            result.ErrorType = ErrorType.NotFound;
            return result;
        }

        public async Task<ResultContainer<PhysicalFileResult>> GetById(Guid id)
        {
            var result = new ResultContainer<PhysicalFileResult>();
            var file = await _fileRepository.GetById(id);
            if (file == null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var fileInfo = new FileInfo(file.Path);

            if (fileInfo.Exists)
            {
                /*var fileUploadResponse = new FileUploadResponseDto
                {
                    Bytes = await System.IO.File.ReadAllBytesAsync(file.Path)
                };
                result = _mapper.Map<ResultContainer<FileUploadResponseDto>>(fileUploadResponse);
                return result;*/


                /*var fileUpload = new FileUploadResponseDto();
                fileUpload.PhysicalFileResult = GetFileResult(file.Path,file.Types, file.Name.ToString());
                result = _mapper.Map<ResultContainer<FileUploadResponseDto>>(fileUpload); */

                var fileResult = GetFileResult(file.Path, file.Types, file.Name.ToString());

                result = _mapper.Map<ResultContainer<PhysicalFileResult>>(fileResult);
                return result;
            }

            result.ErrorType = ErrorType.NotFound;
            return result;
        }

        public async Task<ResultContainer<FileModelResponseDto>> Delete(Guid id)
        {
            var result = new ResultContainer<FileModelResponseDto>();
            var file = await _fileRepository.Delete(id);
            if (file == null)
            {
                result.ErrorType = ErrorType.NotFound;
                return result;
            }

            var fileInfo = new FileInfo(file.Path);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            result.ContentResult = ContentResult.NoContentResult;

            return result;
        }

        private static PhysicalFileResult GetFileResult(string path, string types, string name)
        {
            return new PhysicalFileResult(path, "application/octet-stream")
            {
                FileDownloadName = name + types
            };
        }
    }
}