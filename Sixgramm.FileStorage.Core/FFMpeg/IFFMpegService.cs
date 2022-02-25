using System.Threading.Tasks;

namespace Sixgramm.FileStorage.Core.FFMpeg;

public interface IFFMpegService
{
    public Task ConvertingVideoHd(string inputPath, string outputPath);
    /*public void ConvertingVideoEd(string inputPath, string outputPath);
    public void ConvertingVideoLd(string inputPath, string outputPath);
    public void ConvertingVideoFullHd(string inputPath, string outputPath);*/

}