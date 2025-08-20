using sls_borders.DTO.ImageDto;
using sls_borders.Enums;

namespace sls_borders.Repositories
{
    /// <summary>
    /// Interface for image upload and management service
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Uploads an image file to the appropriate category folder
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="category">The category (Avatar or Image)</param>
        /// <returns>Result containing file information and URL</returns>
        Task<ImageUploadResultDto> UploadImageAsync(Stream fileStream, string fileName, string contentType, ImageCategory category);
        
        /// <summary>
        /// Deletes an image file
        /// </summary>
        /// <param name="fileName">The name of the file to delete</param>
        /// <param name="category">The category of the image</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteImageAsync(string fileName, ImageCategory category);
        
        /// <summary>
        /// Gets the URL for an image
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="category">The category of the image</param>
        /// <returns>The URL to access the image</returns>
        string GetImageUrl(string fileName, ImageCategory category);
        
        /// <summary>
        /// Gets the physical path for an image
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="category">The category of the image</param>
        /// <returns>The physical path to the image file</returns>
        string GetImagePath(string fileName, ImageCategory category);
    }
}
