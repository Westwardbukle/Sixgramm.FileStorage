using System;

namespace Sixgramm.FileStorage.Core.SaveFile;

public interface IFilePathService
{
    public void SetVideoPath(string type, string fileSource, string sourceId,
        out string firstPath, out string outputPath, out Guid name, out Guid videoName720);

    public void SetAvtarPath(string type, string fileSource, out string firstPath, out Guid name);
    public void SetFilePath(string type, string fileSource, Guid sourceId, out string firstPath, out Guid name);
}