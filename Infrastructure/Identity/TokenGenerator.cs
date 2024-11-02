using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth;

internal class TokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _singningCredentials;

    public TokenGenerator(IOptions<JwtOptions> jwtOptions, JwtSecurityTokenHandler tokenHandler)
    {
        _jwtOptions = jwtOptions.Value;
        _tokenHandler = tokenHandler;

        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        _singningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
    }

    public string GenerateToken(Guid userId, string email)
    {
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString("d")),
            new Claim(JwtRegisteredClaimNames.Email, email)
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = _singningCredentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
}
