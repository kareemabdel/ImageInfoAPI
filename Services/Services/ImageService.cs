
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Services.Contracts;
using Services.Helpers;
using System.Drawing;
using System.Text.Json;


namespace Services.Services
{
    public class ImageService : IImageService
    {
        private readonly StorageSettings _settings;
        private readonly ILogger<ImageService> _logger;  
        public ImageService(IOptions<StorageSettings> options, ILogger<ImageService> logger)
        {
            _settings = options.Value;
            _logger = logger;
            System.IO.Directory.CreateDirectory(Path.Combine(_settings.BasePath, _settings.OriginalFolder));
            System.IO.Directory.CreateDirectory(Path.Combine(_settings.BasePath,_settings.PhoneFolder));
            System.IO.Directory.CreateDirectory(Path.Combine(_settings.BasePath,_settings.TabletFolder));
            System.IO.Directory.CreateDirectory(Path.Combine(_settings.BasePath,_settings.DesktopFolder));
            System.IO.Directory.CreateDirectory(Path.Combine(_settings.BasePath, _settings.MetadataFolder));
        }

        public async Task<string> UploadAndProcessImageAsync(IFormFile file)
        {
            try
            {
                ValidationHelper.ValidateFile(file);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Attempted to upload invalid file: {file.FileName}");
                throw; 
            }

            var uniqueId = Guid.NewGuid().ToString();

            var originalPath = Path.Combine(_settings.BasePath, _settings.OriginalFolder, $"{uniqueId}.webp");

            using var image = await Image.LoadAsync(file.OpenReadStream());

            await image.SaveAsWebpAsync(originalPath); // Convert to WebP

            await ResizeAndSaveAsync(image, uniqueId);

            var metadata = ExtractMetadata(file);
            metadata.UniqueId = uniqueId;

            var metadataPath = Path.Combine(_settings.BasePath, _settings.MetadataFolder, $"{uniqueId}.json");
            await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata));
            _logger.LogInformation($"Image {uniqueId} uploaded successfully.");
            return uniqueId;
        }

        private async Task ResizeAndSaveAsync(Image image, string uniqueId)
        {
            var phoneSize = new SixLabors.ImageSharp.Size(480, 800);
            var tabletSize = new SixLabors.ImageSharp.Size(768, 1024);
            var desktopSize = new SixLabors.ImageSharp.Size(1920, 1080);

            await SaveResizedImage(image, phoneSize, Path.Combine(_settings.BasePath, $"Resized/{_settings.PhoneFolder}", $"{uniqueId}.webp"));
            await SaveResizedImage(image, tabletSize, Path.Combine(_settings.BasePath, $"Resized/{_settings.TabletFolder}", $"{uniqueId}.webp"));
            await SaveResizedImage(image, desktopSize, Path.Combine(_settings.BasePath, $"Resized/{_settings.DesktopFolder}", $"{uniqueId}.webp"));
        }

        private async Task SaveResizedImage(Image image, SixLabors.ImageSharp.Size size, string outputPath)
        {
            using var clone = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Max
            }));
            await clone.SaveAsWebpAsync(outputPath);
        }

        private ImageMetadataModel ExtractMetadata(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var directories = ImageMetadataReader.ReadMetadata(stream);

            var gps = directories.OfType<GpsDirectory>().FirstOrDefault();
            var exif = directories.OfType<ExifIfd0Directory>().FirstOrDefault();

            double? latitude = null;
            double? longitude = null;

            if (gps != null)
            {
                var loc = gps.GetGeoLocation();
                latitude = loc?.Latitude;
                longitude = loc?.Longitude;
            }

            return new ImageMetadataModel
            {
                CameraMake = exif?.GetDescription(ExifDirectoryBase.TagMake),
                CameraModel = exif?.GetDescription(ExifDirectoryBase.TagModel),
                Latitude = latitude,
                Longitude = longitude
            };
        }

        public async Task<FileStreamResult> DownloadImageAsync(string uniqueId, string size)
        {
            var path = Path.Combine(_settings.BasePath, "Resized", size, $"{uniqueId}.webp");
            if (!File.Exists(path))
                throw new FileNotFoundException("Image not found.");

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, "image/webp");
        }

        public async Task<ImageMetadataModel> GetMetadataAsync(string uniqueId)
        {
            var path = Path.Combine(_settings.BasePath, _settings.MetadataFolder, $"{uniqueId}.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("Metadata not found.");

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<ImageMetadataModel>(json);
        }


    }

}
