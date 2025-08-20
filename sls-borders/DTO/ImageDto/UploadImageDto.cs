using sls_borders.Enums;

namespace sls_borders.DTO.ImageDto
{
    /// <summary>
    /// DTO for image upload request
    /// </summary>
    public class UploadImageRequestDto
    {
        /// <summary>
        /// The category of the image (Avatar or Image)
        /// </summary>
        public ImageCategory Category { get; set; }
    }
}
