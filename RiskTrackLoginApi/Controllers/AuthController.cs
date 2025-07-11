using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using Microsoft.IdentityModel.Tokens;
using RiskTrackLoginApi.Data;
using RiskTrackLoginApi.DTOs;
using RiskTrackLoginApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RiskTrackLoginApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        private static Dictionary<string, string> CodeStorage = new();

        public AuthController(ApplicationDbContext context, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Credenciales inválidas.");

            var code = new Random().Next(100000, 999999).ToString();
            CodeStorage[dto.Email] = code;

            await _emailService.SendVerificationEmailAsync(user.Email!, code);

            return Ok(new { message = "Código de verificación enviado", email = user.Email });
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify(VerifyDto dto)
        {
            if (!CodeStorage.TryGetValue(dto.Email, out var storedCode) || storedCode != dto.Code)
                return Unauthorized("Código inválido o expirado.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound("Usuario no encontrado.");

            var jwtConfig = _config.GetSection("Jwt");
            var claims = new[]
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim("companyId", user.CompanyId?.ToString() ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtConfig["ExpireMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenStr,
                userId = user.UserId,
                companyId = user.CompanyId,
                role = user.Role
            });
        }
    }
}

