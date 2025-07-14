namespace sls_borders.DTO.UserDto;

public class InviteUserDto
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Role { get; set; } = "User";
    public Guid TeamId { get; set; } = Guid.Empty;
}
