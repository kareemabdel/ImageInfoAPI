using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace ImageInfoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        { 
                var result = new List<string>();

                foreach (var file in files)
                {
                    var id = await _imageService.UploadAndProcessImageAsync(file);
                    result.Add(id);
                }

                return Ok(result);
           
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string uniqueId, [FromQuery] string size)
        {
            var file = await _imageService.DownloadImageAsync(uniqueId, size);
            return file;
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadata([FromQuery] string uniqueId)
        {
            var metadata = await _imageService.GetMetadataAsync(uniqueId);
            return Ok(metadata);
        }
    }
}
