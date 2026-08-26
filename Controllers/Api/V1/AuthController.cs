using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberFlow.Application.API;
using BarberFlow.Application.DTOs.Requests;
using BarberFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BarberFlow.Controllers.Api.V1;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    { _userManager = userManager; _configuration = configuration; }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginApiRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Senha))
            return Unauthorized(ApiResponse<object>.Fail("E-mail ou senha inválidos."));

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationMinutes", 120));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"],
            claims: claims, expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return Ok(ApiResponse<object>.Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt = expires,
            roles
        }, "Autenticação realizada com sucesso."));
    }
}
