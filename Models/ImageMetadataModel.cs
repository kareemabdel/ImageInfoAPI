using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ImageMetadataModel
    {
        public string UniqueId { get; set; }
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
