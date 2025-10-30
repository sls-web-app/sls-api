using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sls_borders.Models
{
    /// <summary>
    /// Represents an Image metadata
    /// </summary>
    public class Image
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
