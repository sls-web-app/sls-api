using sls_borders.Enums;

namespace sls_borders.DTO.ImageDto
{
    /// <summary>
    /// DTO for image upload request
    /// </summary>
    public class UploadImageRequestDto
    {
        public ImageCategory Category { get; set; }
    }
}
