using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.ImageDto;
using sls_borders.Enums;
using sls_borders.Repositories;

namespace sls_api.Controllers;

/// <summary>
/// API controller for image upload and management.
/// Provides endpoints for uploading, deleting, and accessing images.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ImageController(IImageService imageService) : ControllerBase
{
    /// <summary>
    /// Uploads an image file
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="category">The category of the image (avatar or image)</param>
    /// <returns>Upload result with file information</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImageUploadResultDto>> UploadImage(IFormFile file, [FromForm] ImageCategory category)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ImageUploadResultDto
            {
                Success = false,
                ErrorMessage = "No file was uploaded"
            });
        }

        using var stream = file.OpenReadStream();
        var result = await imageService.UploadImageAsync(stream, file.FileName, file.ContentType, category);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Deletes an image file
    /// </summary>
    /// <param name="fileName">The name of the file to delete</param>
    /// <param name="category">The category of the image</param>
    /// <returns>Success status</returns>
    [HttpDelete("{category}/{fileName}")]
    [Authorize]
    public async Task<ActionResult<bool>> DeleteImageAsync(string fileName, ImageCategory category)
    {
        var result = await imageService.DeleteImageAsync(fileName, category);

        if (result)
        {
            return Ok(new { success = true, message = "Image deleted successfully" });
        }

        return NotFound(new { success = false, message = "Image not found" });
    }

    /// <summary>
    /// Gets the URL for an image
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <param name="category">The category of the image</param>
    /// <returns>The image URL</returns>
    [HttpGet("{category}/{fileName}/url")]
    public ActionResult<string> GetImageUrl(string fileName, ImageCategory category)
    {
        var url = imageService.GetImageUrl(fileName, category);
        return Ok(new { url });
    }

    /// <summary>
    /// Serves the actual image file
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <param name="category">The category of the image</param>
    /// <returns>The image file</returns>
    [HttpGet("{category}/{fileName}")]
    public ActionResult GetImage(string fileName, ImageCategory category)
    {
        var filePath = imageService.GetImagePath(fileName, category);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "Image not found" });
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var contentType = GetContentType(filePath);

        return File(fileBytes, contentType);
    }

    /// <summary>
    /// Gets the content type based on file extension
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>Content type string</returns>
    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
