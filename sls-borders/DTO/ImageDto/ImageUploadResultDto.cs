namespace sls_borders.DTO.ImageDto
{
    /// <summary>
    /// Response DTO for image upload operations
    /// </summary>
    public class ImageUploadResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
