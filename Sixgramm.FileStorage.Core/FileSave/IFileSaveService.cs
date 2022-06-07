using System.Threading.Tasks;
using Sixgramm.FileStorage.Core.Dto.FileInfo;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Core.FileSave;

public interface IFileSaveService
{
    public Task<FileModel> SaveAvatar(string type, string fileSource, FileInfoModuleDto fileInfoModuleDto);

    public Task<FileModel> SaveVideoFile(string type, string fileSource, string sourceId,
        FileInfoModuleDto fileInfoModuleDto);

    public Task<FileModel> SaveFile(string type, string fileSource, FileInfoModuleDto fileInfoModuleDto);
}