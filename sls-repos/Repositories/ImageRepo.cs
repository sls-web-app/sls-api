using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.ImageDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories;

/// <summary>
/// Repository for managing Image metadata in the database
/// </summary>
public class ImageRepo(ApplicationDbContext context) : IImageRepo
{
    public async Task<List<Image>> GetAllAsync()
    {
        return await context.Images.ToListAsync();
    }

    public async Task<Image?> GetByIdAsync(Guid id)
    {
        return await context.Images.FindAsync(id);
    }

    public async Task<Image?> GetByFileNameAsync(string fileName)
    {
        return await context.Images
          .FirstOrDefaultAsync(i => i.FileName == fileName);
    }

    public async Task<Image> CreateAsync(CreateImageDto createImageDto)
    {
    var image = new Image
        {
      Id = Guid.NewGuid(),
            Title = createImageDto.Title,
       FileName = createImageDto.FileName,
            ContentType = createImageDto.ContentType,
        UploadedAt = DateTime.UtcNow
      };

     context.Images.Add(image);
        await context.SaveChangesAsync();

        return image;
    }

    public async Task<Image?> UpdateAsync(Guid id, CreateImageDto updateImageDto)
    {
        var existingImage = await context.Images.FindAsync(id);
 
      if (existingImage == null)
            return null;

   existingImage.Title = updateImageDto.Title;
        existingImage.FileName = updateImageDto.FileName;
        existingImage.ContentType = updateImageDto.ContentType;

        await context.SaveChangesAsync();
    return existingImage;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var image = await context.Images.FindAsync(id);
        
    if (image == null)
            return false;

        context.Images.Remove(image);
        await context.SaveChangesAsync();
        return true;
    }
}
