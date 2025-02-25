using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class TokenService(IConfiguration configuration)
{
    public string GenerateJwtToken(Guid userId, string userName)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var stringKey = configuration["JWT_KEY"] 
            ?? throw new InvalidOperationException("JWT_KEY does not exist");
        var key = Encoding.ASCII.GetBytes(stringKey);
        var issuer = configuration["JWT_ISSUER"]
                     ?? throw new InvalidOperationException("JWT_ISSUER does not exist");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = issuer,
            Audience = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}