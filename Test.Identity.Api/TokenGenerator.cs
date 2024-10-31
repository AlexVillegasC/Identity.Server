using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Test.Identity.Api;

public class TokenGenerator
{
    public string GenerateToken(string email, List<string> audiences)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = "ForTheLoveOfGodStoreAndLoadThisSecurely"u8.ToArray();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("admin", "read:write")
        };

        // Generate token with multiple audiences
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://localhost:7014", // Identity Server URL
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // Create a JwtSecurityToken to add multiple audiences
        var jwtToken = new JwtSecurityToken(
            issuer: tokenDescriptor.Issuer,
            claims: tokenDescriptor.Subject.Claims,
            expires: tokenDescriptor.Expires,
            signingCredentials: tokenDescriptor.SigningCredentials,
            audience: null // Set audience to null to avoid single audience restriction
        );

        // Add multiple audiences                
        jwtToken.Payload["aud"] = audiences;


        return tokenHandler.WriteToken(jwtToken);
    }
}
