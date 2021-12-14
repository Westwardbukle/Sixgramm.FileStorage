using Sixgramm.FileStorage.Common.Base;
using Sixgramm.FileStorage.Common.Types;

namespace Sixgramm.FileStorage.Core.Dto.File
{
    public class FileModelDto:BaseModel
    {
        public string Name { get; set; }
        public string Length { get; set; }
        public string Typr { get; set; }
        
        public  FileTypes Types { get; set; }
    }
}