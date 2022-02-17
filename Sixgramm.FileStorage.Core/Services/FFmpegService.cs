using System.Threading.Tasks;
using FFMpegCore.Enums;
using Sixgramm.FileStorage.Core.FFMpeg;

namespace Sixgramm.FileStorage.Core.Services;

public class FFmpegService : IFFMpegService
{
    public void ConvertingVideoHd(string inputPath, string outputPath)
    {
         FFMpegCore.FFMpeg.Convert(inputPath, outputPath, VideoType.Mp4, Speed.Medium, VideoSize.Hd, AudioQuality.Normal,
            true);
    }
    /*public void ConvertingVideoEd(string inputPath, string outputPath)
    {
        FFMpegCore.FFMpeg.Convert(inputPath, outputPath, VideoType.Mp4, Speed.Medium, VideoSize.Ed, AudioQuality.Normal,
            true);
    }
    public void ConvertingVideoLd(string inputPath, string outputPath)
    {
        FFMpegCore.FFMpeg.Convert(inputPath, outputPath, VideoType.Mp4, Speed.Medium, VideoSize.Ld, AudioQuality.Normal,
            true);
    }
    public void ConvertingVideoFullHd(string inputPath, string outputPath)
    {
        FFMpegCore.FFMpeg.Convert(inputPath, outputPath, VideoType.Mp4, Speed.Medium, VideoSize.FullHd, AudioQuality.Normal,
            true);
    }*/
}