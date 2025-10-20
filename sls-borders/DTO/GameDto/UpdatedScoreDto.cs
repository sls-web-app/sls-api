using sls_borders.Enums;

namespace sls_borders.DTO.GameDto;

public class UpdatedScoreDto
{
    public Guid Id { get; set; }
    public GameScore? Score { get; set; }
}