using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixgramm.FileStorage.Common.Result;

namespace Sixgramm.FileStorage.Core.File;

public interface IFileSaveService
{
    string SetFilePath(string type, Guid name);
}