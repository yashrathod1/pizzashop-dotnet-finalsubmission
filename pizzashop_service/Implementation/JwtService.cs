using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using pizzashop_repository.Interface;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;


    public JwtService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;

    }

    public async Task<string> GenerateJwtToken(string email, int roleId)
    {
        string? Rolename = await _userRepository.GetUserRole(roleId);
    
        Claim[]? claims = new[]
        {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, Rolename),
        
        };
        Console.WriteLine($"🔹 JWT Claims: Email={email}, RoleId={Rolename}");


        SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        SigningCredentials? credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken? token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
