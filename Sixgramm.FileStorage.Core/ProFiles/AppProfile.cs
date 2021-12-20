using AutoMapper;
using Sixgramm.FileStorage.Core.Dto.File;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Core.ProFiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<FileModel, FileModelDto>();
            
        }
    }
}