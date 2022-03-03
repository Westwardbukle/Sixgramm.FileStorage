using System.Threading.Tasks;

namespace Sixgramm.FileStorage.Core.FFMpeg;

public interface IFFMpegService
{
    public Task ConvertingVideoHd(string inputPath, string outputPath);
    
}