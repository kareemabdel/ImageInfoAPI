using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public static class ValidationHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

        public static void ValidateFile(IFormFile file)
        {
            if (file.Length == 0)
                throw new Exception("File is empty.");

            if (file.Length > MaxFileSize)
                throw new Exception("File exceeds 2MB limit.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new Exception("Invalid file type. Only JPG, PNG, WEBP are allowed.");
        }
    }

}
