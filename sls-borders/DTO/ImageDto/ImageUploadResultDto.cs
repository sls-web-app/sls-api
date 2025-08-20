namespace sls_borders.DTO.ImageDto
{
    /// <summary>
    /// Response DTO for image upload operations
    /// </summary>
    public class ImageUploadResultDto
    {
        /// <summary>
        /// The generated filename of the uploaded image
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// The full file path where the image is stored
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
        
        /// <summary>
        /// The URL to access the uploaded image
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Indicates if the upload was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Error message if upload failed
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
