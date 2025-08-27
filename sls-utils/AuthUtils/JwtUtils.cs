using Microsoft.IdentityModel.Tokens;
using sls_borders.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace sls_utils.AuthUtils;

public class JwtUtils
{
    public static string GenerateJwtToken(Guid id, string username, Role role, string keyString)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, id.ToString()),
            new (ClaimTypes.Name, username),
            new (ClaimTypes.Role, role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials
        };

        var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}