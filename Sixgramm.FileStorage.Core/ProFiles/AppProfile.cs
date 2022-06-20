using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Core.ProFiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<FileModel, FileModelResponseDto>();

            CreateMap<FileModel, ResultContainer<FileModelResponseDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));

            CreateMap<FileModel, FileDownloadResponseDto>();

            CreateMap<FileModel, ResultContainer<FileDownloadResponseDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));

            CreateMap<PhysicalFileResult, ResultContainer<PhysicalFileResult>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));

            CreateMap<FileModel, ResultContainer<FileInfoDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));

            CreateMap<FileModel, FileInfoDto>()
                .ForMember("FilePath", opt =>
                    opt.MapFrom(f => f.Path))
                .ForMember("FileName", opt =>
                    opt.MapFrom(f => f.Name.ToString()))
                .ForMember("FileType", opt =>
                    opt.MapFrom(f => f.Types));

            /*CreateMap<FileModel, FileInfoDto>()
                .ForMember("FileName", opt =>
                    opt.MapFrom(f => f.Name.ToString()));

            CreateMap<FileModel, FileInfoDto>()
                .ForMember("FileType", opt =>
                    opt.MapFrom(f => f.Types));*/
        }
    }
}