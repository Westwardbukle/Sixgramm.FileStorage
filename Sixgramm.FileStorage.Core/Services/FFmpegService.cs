using System;
using System.Drawing;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using Sixgramm.FileStorage.Core.FFMpeg;

namespace Sixgramm.FileStorage.Core.Services;

public class FFmpegService : IFFMpegService
{
    public async Task ConvertingVideoHd(string inputPath, string outputPath)
    {
        await FFMpegArguments
            .FromFileInput(inputPath)
            .OutputToFile(outputPath, false, options => options
                .WithVideoCodec(VideoCodec.LibX264)
                .WithConstantRateFactor(21)
                .WithAudioCodec(AudioCodec.Aac)
                .WithVariableBitrate(4)
                .Resize(1280, 720)
                .WithVideoFilters(filterOptions => filterOptions
                    .Scale(VideoSize.Hd))
                .WithSpeedPreset(Speed.UltraFast))
            .ProcessAsynchronously();
    }
}