using AutoMapper;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.Dto.File;
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
            CreateMap<FileModel, FileDownloadResponseDto>();
            CreateMap<FileModel, ResultContainer<FileModelResponseDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));
            CreateMap<FileModel, ResultContainer<FileDownloadResponseDto>>()
                .ForMember("Data", opt =>
                    opt.MapFrom(f => f));
            CreateMap<TokenModel, FileModelDto>();
        }
    }
}