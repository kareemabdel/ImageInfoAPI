using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class StorageSettings
    {
        public string BasePath { get; set; }
        public string OriginalFolder { get; set; }
        public string PhoneFolder { get; set; }
        public string TabletFolder { get; set; }
        public string DesktopFolder { get; set; }
        public string MetadataFolder { get; set; }
    }
}
