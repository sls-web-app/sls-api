using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sls_borders.DTO.ImageDto;
using sls_borders.Enums;
using sls_borders.Repositories;

namespace sls_utils.ImageUtils
{
    /// <summary>
    /// Service for handling image uploads and management
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageService> _logger;
        private readonly string _uploadPath;
        private readonly string _baseUrl;

        /// <summary>
        /// Constructor for ImageService
        /// </summary>
        /// <param name="configuration">Configuration service</param>
        /// <param name="logger">Logger service</param>
        public ImageService(IConfiguration configuration, ILogger<ImageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // Get upload path from configuration or use default
            _uploadPath = _configuration["ImageUpload:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            _baseUrl = _configuration["ImageUpload:BaseUrl"] ?? "http://localhost:8080/uploads";
            
            // Ensure upload directories exist
            EnsureDirectoriesExist();
        }

        /// <summary>
        /// Uploads an image file to the appropriate category folder
        /// </summary>
        /// <param name="fileStream">The image file stream</param>
        /// <param name="fileName">Original file name</param>
        /// <param name="contentType">Content type of the file</param>
        /// <param name="category">The category (Avatar or Image)</param>
        /// <returns>Result containing file information and URL</returns>
        public async Task<ImageUploadResultDto> UploadImageAsync(Stream fileStream, string fileName, string contentType, ImageCategory category)
        {
            try
            {
                // Validate file
                if (fileStream == null || fileStream.Length == 0)
                {
                    return new ImageUploadResultDto
                    {
                        Success = false,
                        ErrorMessage = "File is empty or null"
                    };
                }

                // Validate content type
                if (!IsValidImageContentType(contentType))
                {
                    return new ImageUploadResultDto
                    {
                        Success = false,
                        ErrorMessage = "Invalid file type. Only JPG, PNG, GIF, and WebP images are allowed."
                    };
                }

                // Validate file size (max 5MB)
                const long maxFileSize = 5 * 1024 * 1024;
                if (fileStream.Length > maxFileSize)
                {
                    return new ImageUploadResultDto
                    {
                        Success = false,
                        ErrorMessage = "File size exceeds 5MB limit"
                    };
                }

                // Generate unique filename
                var extension = Path.GetExtension(fileName);
                var uniqueFileName = GenerateUniqueFileName(category, extension);
                
                // Get category folder path
                var categoryFolder = GetCategoryFolder(category);
                var fullPath = Path.Combine(_uploadPath, categoryFolder);
                var filePath = Path.Combine(fullPath, uniqueFileName);

                // Ensure directory exists
                Directory.CreateDirectory(fullPath);

                // Save file
                using var fileStreamOut = new FileStream(filePath, FileMode.Create);
                await fileStream.CopyToAsync(fileStreamOut);

                _logger.LogInformation("Image uploaded successfully: {FileName}", uniqueFileName);

                return new ImageUploadResultDto
                {
                    FileName = uniqueFileName,
                    FilePath = filePath,
                    ImageUrl = GetImageUrl(uniqueFileName, category),
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", fileName);
                return new ImageUploadResultDto
                {
                    Success = false,
                    ErrorMessage = $"Error uploading image: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Deletes an image file
        /// </summary>
        /// <param name="fileName">The name of the file to delete</param>
        /// <param name="category">The category of the image</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteImageAsync(string fileName, ImageCategory category)
        {
            try
            {
                var filePath = GetImagePath(fileName, category);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Image deleted successfully: {FileName}", fileName);
                    return true;
                }

                _logger.LogWarning("Image file not found for deletion: {FileName}", fileName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {FileName}", fileName);
                return false;
            }
        }

        /// <summary>
        /// Gets the URL for an image
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="category">The category of the image</param>
        /// <returns>The URL to access the image</returns>
        public string GetImageUrl(string fileName, ImageCategory category)
        {
            var categoryFolder = GetCategoryFolder(category);
            return $"{_baseUrl.TrimEnd('/')}/{categoryFolder}/{fileName}";
        }

        /// <summary>
        /// Gets the physical path for an image
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="category">The category of the image</param>
        /// <returns>The physical path to the image file</returns>
        public string GetImagePath(string fileName, ImageCategory category)
        {
            var categoryFolder = GetCategoryFolder(category);
            return Path.Combine(_uploadPath, categoryFolder, fileName);
        }

        /// <summary>
        /// Generates a unique filename based on UTC time, GUID, and category
        /// </summary>
        /// <param name="category">Image category</param>
        /// <param name="extension">File extension</param>
        /// <returns>Unique filename</returns>
        private static string GenerateUniqueFileName(ImageCategory category, string extension)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8]; // First 8 characters of GUID
            var categoryPrefix = category.ToString().ToLower();
            
            return $"{categoryPrefix}_{timestamp}_{guid}{extension}";
        }

        /// <summary>
        /// Gets the folder name for a category
        /// </summary>
        /// <param name="category">Image category</param>
        /// <returns>Folder name</returns>
        private static string GetCategoryFolder(ImageCategory category)
        {
            return category switch
            {
                ImageCategory.Avatar => "avatars",
                ImageCategory.Image => "images",
                _ => "images"
            };
        }

        /// <summary>
        /// Validates if the content type is a valid image type
        /// </summary>
        /// <param name="contentType">Content type to validate</param>
        /// <returns>True if valid image content type</returns>
        private static bool IsValidImageContentType(string contentType)
        {
            var validTypes = new[]
            {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/gif",
                "image/webp"
            };

            return validTypes.Contains(contentType.ToLower());
        }

        /// <summary>
        /// Ensures upload directories exist
        /// </summary>
        private void EnsureDirectoriesExist()
        {
            try
            {
                var avatarsPath = Path.Combine(_uploadPath, "avatars");
                var imagesPath = Path.Combine(_uploadPath, "images");

                Directory.CreateDirectory(avatarsPath);
                Directory.CreateDirectory(imagesPath);

                _logger.LogInformation("Upload directories ensured: {AvatarsPath}, {ImagesPath}", avatarsPath, imagesPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating upload directories");
                throw;
            }
        }
    }
}
