using System;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Enums;
using Sixgramm.FileStorage.Core.FFMpeg;

namespace Sixgramm.FileStorage.Core.Services;

public class FFmpegService : IFFMpegService
{
    public async Task ConvertingVideoHd(string inputPath, string outputPath)
    {
        await FFMpegArguments
            .FromFileInput(inputPath)
            .OutputToFile(outputPath, true, options => options
                .WithVideoCodec(VideoCodec.LibX264)
                .WithConstantRateFactor(21)
                .WithAudioCodec(AudioCodec.Aac)
                .WithVariableBitrate(4)
                .WithVideoFilters(filterOptions => filterOptions
                    .Scale(VideoSize.Hd)))
            .ProcessAsynchronously();
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