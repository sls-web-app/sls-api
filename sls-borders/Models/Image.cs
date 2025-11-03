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
        public string Description { get; set; } = null!;
        public int Status { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public Guid TournamentId { get; set; } = Guid.Empty;
        public Guid EditionId { get; set; } = Guid.Empty;

        public Tournament Tournament { get; set; } = null!;
        public Edition Edition { get; set; } = null!;
    }
}
