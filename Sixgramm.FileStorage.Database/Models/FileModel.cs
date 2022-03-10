using System;
using Sixgramm.FileStorage.Common.Base;
using Sixgramm.FileStorage.Common.Types;

namespace Sixgramm.FileStorage.Database.Models
{
    public class FileModel : BaseModel
    {
        public Guid Name { get; set; }
        public Guid UserId { get; set; }
        public string Path { get; set; }
        public long Length { get; set; }
        public string Types { get; set; }
        public Guid SourceId { get; set; }
        public string FileSource { get; set; }
    }
}