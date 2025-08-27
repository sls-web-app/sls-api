using sls_borders.DTO.EditionDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IEditionRepo
    {
        Task<List<Edition>> GetAllAsync();
        Task<Edition> CreateAsync(CreateEditionDto newEdition);
        Task<Edition?> GetByIdAsync(Guid id);
    }
}
