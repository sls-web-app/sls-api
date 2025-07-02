using Microsoft.IdentityModel.Tokens;
using sls_borders.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace sls_api.Utils;

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

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}