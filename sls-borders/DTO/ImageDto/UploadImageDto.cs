using sls_borders.Enums;

namespace sls_borders.DTO.ImageDto
{
    /// <summary>
    /// DTO for image upload request
    /// </summary>
    public class UploadImageRequestDto
    {
        public ImageCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public Guid EditionId { get; set; }
        public Guid TournamentId { get; set; }
    }
}
