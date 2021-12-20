using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.Repository.Base;

namespace Sixgramm.FileStorage.Database.Repository.File
{
    public class FileRepository : BaseRepository<FileModel>, IFileRepository
    {
        public FileRepository(AppDbContext context) : base(context)
        {
        }
    }
}