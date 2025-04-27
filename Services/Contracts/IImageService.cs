using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IImageService
    {
        Task<string> UploadAndProcessImageAsync(IFormFile file);
        Task<FileStreamResult> DownloadImageAsync(string uniqueId, string size);
        Task<ImageMetadataModel> GetMetadataAsync(string uniqueId);
    }
}
