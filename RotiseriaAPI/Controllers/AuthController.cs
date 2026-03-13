using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens; // <-- Esta te falta
using System.IdentityModel.Tokens.Jwt; // <-- Esta te falta
using System.Security.Claims;
using System.Text; // <-- Esta para Encoding
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) => _config = config;

    [HttpPost("login")]
    public IActionResult Login([FromBody] User login)
    {
        // 1. Simulación de validación (Después lo haremos contra la DB)
        if (login.Username == "admin" && login.PasswordHash == "1234")
        {
            var token = GenerateToken("admin", "Admin");
            return Ok(new { token });
        }

        return Unauthorized("Usuario o clave incorrectos");
    }

    private string GenerateToken(string username, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Audience"],
          claims,
          expires: DateTime.Now.AddHours(8), // El turno de la rotisería
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}