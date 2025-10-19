using sls_borders.DTO.TeamDto;
using sls_borders.Enums;

public class UserInPlay
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string ProfileImg { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public Role Role { get; set; }
    public bool AccountActivated { get; set; }
    public bool IsInPlay { get; set; }
    public bool IsLider { get; set; }

    public GetTeamDto? Team { get; set; }

    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
}