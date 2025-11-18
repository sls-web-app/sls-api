using AutoMapper;
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
public class ImageController(IImageService imageService, IImageRepo imageRepo, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Uploads an image file
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="uploadRequest">Upload request containing image metadata</param>
    /// <returns>Upload result with file information</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImageUploadResultDto>> UploadImage(
        IFormFile file,
        [FromForm] UploadImageRequestDto uploadRequest)
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
        var result = await imageService.UploadImageAsync(stream, file.FileName, file.ContentType, uploadRequest.Category);

        if (result.Success)
        {
            // If category is Image, also save metadata to database
            if (uploadRequest.Category == ImageCategory.Image)
            {
                if (uploadRequest.EditionId == Guid.Empty || uploadRequest.TournamentId == Guid.Empty)
                {
                    return BadRequest(new ImageUploadResultDto
                    {
                        Success = false,
                        ErrorMessage = "EditionId and TournamentId are required for Image category"
                    });
                }

                var createImageDto = new CreateImageDto
                {
                    Title = string.IsNullOrWhiteSpace(uploadRequest.Title) 
                        ? Path.GetFileNameWithoutExtension(file.FileName) 
                        : uploadRequest.Title,
                    Description = uploadRequest.Description,
                    FileName = result.FileName,
                    ContentType = file.ContentType,
                    EditionId = uploadRequest.EditionId,
                    TournamentId = uploadRequest.TournamentId
                };

                await imageRepo.CreateAsync(createImageDto);
            }

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
    public async Task<ActionResult<bool>> DeleteImageAsync(string fileName, ImageCategory category)
    {
        var result = await imageService.DeleteImageAsync(fileName, category);

        if (result)
        {
            // If category is Image, also delete metadata from database
            if (category == ImageCategory.Image)
            {
                var imageMetadata = await imageRepo.GetByFileNameAsync(fileName);
                if (imageMetadata != null)
                {
                    await imageRepo.DeleteAsync(imageMetadata.Id);
                }
            }

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

    // ==================== IMAGE METADATA ENDPOINTS ====================

    /// <summary>
    /// Gets all image metadata records
    /// </summary>
    /// <returns>List of all image metadata</returns>
    [HttpGet("metadata/get-all")]
    public async Task<ActionResult<List<GetImageDto>>> GetAllImageMetadata()
    {
        var images = await imageRepo.GetAllAsync();
        var imageDtos = mapper.Map<List<GetImageDto>>(images);
        return Ok(imageDtos);
    }

    /// <summary>
    /// Gets image metadata by ID
    /// </summary>
    /// <param name="id">The image ID</param>
    /// <returns>Image metadata</returns>
    [HttpGet("metadata/{id}")]
    public async Task<ActionResult<GetImageDto>> GetImageMetadataById(Guid id)
    {
        var image = await imageRepo.GetByIdAsync(id);

        if (image == null)
        {
            return NotFound(new { message = "Image metadata not found" });
        }

        var imageDto = mapper.Map<GetImageDto>(image);
        return Ok(imageDto);
    }

    /// <summary>
    /// Gets image metadata by filename
    /// </summary>
    /// <param name="fileName">The filename</param>
    /// <returns>Image metadata</returns>
    [HttpGet("metadata/filename/{fileName}")]
    public async Task<ActionResult<GetImageDto>> GetImageMetadataByFileName(string fileName)
    {
        var image = await imageRepo.GetByFileNameAsync(fileName);

        if (image == null)
        {
            return NotFound(new { message = "Image metadata not found" });
        }

        var imageDto = mapper.Map<GetImageDto>(image);
        return Ok(imageDto);
    }

    /// <summary>
    /// Updates image metadata (title only)
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="updateImageDto">Updated image metadata</param>
    /// <returns>Updated image metadata</returns>
    [HttpPut("metadata/{id}")]
    public async Task<ActionResult<GetImageDto>> UpdateImageMetadata(Guid id, [FromBody] CreateImageDto updateImageDto)
    {
        var image = await imageRepo.UpdateAsync(id, updateImageDto);

        if (image == null)
        {
            return NotFound(new { message = "Image metadata not found" });
        }

        var imageDto = mapper.Map<GetImageDto>(image);
        return Ok(imageDto);
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
