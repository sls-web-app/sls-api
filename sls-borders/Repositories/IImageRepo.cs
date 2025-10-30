using sls_borders.DTO.ImageDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    /// <summary>
    /// Interface for Image metadata repository operations
    /// </summary>
  public interface IImageRepo
    {
        /// <summary>
  /// Gets all image metadata records
        /// </summary>
      /// <returns>List of all images</returns>
        Task<List<Image>> GetAllAsync();

        /// <summary>
        /// Gets image metadata by ID
   /// </summary>
        /// <param name="id">Image ID</param>
        /// <returns>Image metadata if found, null otherwise</returns>
        Task<Image?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets image metadata by filename
      /// </summary>
   /// <param name="fileName">File name</param>
     /// <returns>Image metadata if found, null otherwise</returns>
   Task<Image?> GetByFileNameAsync(string fileName);

        /// <summary>
    /// Creates new image metadata record
        /// </summary>
      /// <param name="createImageDto">Image metadata to create</param>
        /// <returns>Created image metadata</returns>
        Task<Image> CreateAsync(CreateImageDto createImageDto);

        /// <summary>
        /// Updates image metadata
        /// </summary>
    /// <param name="id">Image ID</param>
        /// <param name="updateImageDto">Updated image data</param>
   /// <returns>Updated image metadata if found, null otherwise</returns>
   Task<Image?> UpdateAsync(Guid id, CreateImageDto updateImageDto);

        /// <summary>
        /// Deletes image metadata
        /// </summary>
    /// <param name="id">Image ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
