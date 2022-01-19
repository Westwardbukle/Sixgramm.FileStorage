using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Core.Dto.Upload;
using Sixgramm.FileStorage.Core.Token;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Core.ProFiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<FileModel, FileModelDto>();
            CreateMap<FileModel, FileModelResponseDto>();
            CreateMap<FileModel, ResultContainer<FileModelResponseDto>>()
                            .ForMember("Data", opt =>
                                opt.MapFrom(f => f));
            CreateMap<FileModel, FileDownloadResponseDto>();
            CreateMap<FileModel, ResultContainer<FileDownloadResponseDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));
            CreateMap<FileUploadResponseDto, ResultContainer<FileUploadResponseDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));
            
            
            CreateMap<IActionResult, ResultContainer<PhysicalFileResult>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));
            
            
            CreateMap<TokenModel, FileModelDto>();
        }
    }
}