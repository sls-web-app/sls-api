using System.Security.Cryptography;
using System.Text;

namespace sls_utils.AuthUtils;

public class HashingUtils
{
    public static (string Hash, string Salt) HashPassword(string password, string? salt = null)
    {
        HMACSHA512 hmac;
        if(salt != null)
        {
            hmac = new HMACSHA512(Convert.FromBase64String(salt));
        }
        else
        {
            hmac = new HMACSHA512();
        }
        var saltBytes = hmac.Key;
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = hmac.ComputeHash(passwordBytes);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

}