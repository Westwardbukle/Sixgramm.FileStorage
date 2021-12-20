
using System;
using Sixgramm.FileStorage.Common.Base;
using Sixgramm.FileStorage.Common.Types;

namespace Sixgramm.FileStorage.Database.Models
{
    public class FileModel : BaseModel
    {
        public Guid Name { get; set; }
        public string Path { get; set; }
        public long Length { get; set; }
        
        //Оно мне нада?
        
        //public int UserId { get; set; }
        
        public FileTypes Types { get; set; }
    }
}