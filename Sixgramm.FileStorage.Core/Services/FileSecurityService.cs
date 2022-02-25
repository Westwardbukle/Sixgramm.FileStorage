using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sixgramm.FileStorage.Common.Error;
using Sixgramm.FileStorage.Common.Result;
using Sixgramm.FileStorage.Core.Dto.Download;
using Sixgramm.FileStorage.Core.FileSecurity;

namespace Sixgramm.FileStorage.Core.Services;

public class FileSecurityService : IFileSecurityService
{
    private readonly string[] _permittedExtensions;
    
    public FileSecurityService
    (
        IConfiguration configuration
    )
    {
        _permittedExtensions = configuration.GetValue<string>("Extensions").Split(",");
    }
    
    
    private static Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            { ".doc", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                }
            },
            { ".docx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                }
            },
            { ".xls", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1  }
                }
            },
            { ".xlsx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                }
            },
            { ".mp3", new List<byte[]>
                {
                    new byte[] { 0x49, 0x44, 0x33 }
                }
            },
            { ".gif", new List<byte[]>
                {
                    new byte[] { 0x47, 0x49, 0x46, 0x38 }
                }
            },
            { ".png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            },
            { ".mp4", new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70 },
                new byte[] { 0x6D, 0x6D, 0x70, 0x34 },
                new byte[] { 0x6D, 0x64, 0x61, 0x74 },
                new byte[] { 0x00, 0x00, 0x00, 0x1C },
                new byte[] { },
                //new byte[] { 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32 },
            }
        }
        };
    
    private bool CheckExtension(string type)
    {
        return string.IsNullOrEmpty(type) || !_permittedExtensions.Contains(type);
    }

    private bool CheckSignature(IFormFile uploadedFile, string type)
    {
        using var reader = new BinaryReader(uploadedFile.OpenReadStream());
        if(_fileSignature.ContainsKey(type))
        {
            var signatures = _fileSignature[type];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
            if (signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature))==false)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    public bool FileСheck(IFormFile uploadedFile, string type)
        => CheckExtension(type) && CheckSignature(uploadedFile, type);
    
}