using Microsoft.AspNetCore.Mvc;
using ClinicApp.Application.Dtos;
using ClinicApp.Infrastructure.Security;
using ClinicApp.Application.Services;

namespace ClinicApp.Api.Controllers;

/// <summary>
/// Controller responsável por autenticação
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly IPatientService _patientService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(JwtTokenService jwtTokenService, IPatientService patientService, ILogger<AuthController> logger)
    {
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Realizar login e obter token JWT
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("Tentativa de login: {Email}", dto.Email);

        // Usuários de teste (em produção, validar contra banco de dados)
        var validUsers = new[]
        {
            new { Email = "admin@clinicapp.com", Password = "Admin123!" },
            new { Email = "medico@clinicapp.com", Password = "Medico123!" },
            new { Email = "paciente@clinicapp.com", Password = "Paciente123!" },
            new { Email = "moacirsouza@clinicapp.com", Password = "moacir@123456" },
            new { Email = "agente@clinicapp.com", Password = "Agente123!" }
        };

        var user = validUsers.FirstOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);

        if (user == null)
        {
            _logger.LogWarning("Login falhou: credenciais inválidas para {Email}", dto.Email);
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }

        // Tentar buscar o ID real do paciente pelo e-mail
        var patientResult = await _patientService.GetByEmailAsync(dto.Email);
        var userId = patientResult.IsSuccess ? patientResult.Data.Id : 
                     Guid.Parse(dto.Email.Split('@')[0] == "admin" ? "00000000-0000-0000-0000-000000000001" :
                                dto.Email.Split('@')[0] == "medico" ? "00000000-0000-0000-0000-000000000002" :
                                dto.Email.Split('@')[0] == "moacirsouza" ? "00000000-0000-0000-0000-000000000004" :
                                dto.Email.Split('@')[0] == "agente" ? "00000000-0000-0000-0000-000000000005" :
                                "00000000-0000-0000-0000-000000000003");

        var role = dto.Email.Split('@')[0] == "admin" || dto.Email.Split('@')[0] == "moacirsouza" ? "Admin" :
                   dto.Email.Split('@')[0] == "medico" ? "Medico" : 
                   dto.Email.Split('@')[0] == "agente" ? "Agente" : "Paciente";

        var accessToken = _jwtTokenService.GenerateAccessToken(userId, dto.Email, role);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var response = new AuthResponseDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1)
        );

        _logger.LogInformation("Login bem-sucedido: {Email}", dto.Email);

        return Ok(response);
    }

    /// <summary>
    /// Renovar token usando refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult RefreshToken([FromBody] RefreshTokenDto dto)
    {
        _logger.LogInformation("Tentativa de renovação de token");

        // Aqui você validaria o refreshToken contra o banco de dados
        // Por enquanto, vamos apenas gerar um novo token
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(dto.RefreshToken);

        if (principal == null)
        {
            _logger.LogWarning("Refresh token inválido");
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var roleClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(emailClaim) || string.IsNullOrEmpty(roleClaim))
            return Unauthorized(new { message = "Claims inválidos" });

        var accessToken = _jwtTokenService.GenerateAccessToken(Guid.Parse(userIdClaim), emailClaim, roleClaim);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var response = new AuthResponseDto(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddHours(1)
        );

        _logger.LogInformation("Token renovado com sucesso");

        return Ok(response);
    }

    /// <summary>
    /// Registrar novo usuário (para demonstração)
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Register([FromBody] RegisterUserDto dto)
    {
        _logger.LogInformation("Novo registro: {Email}", dto.Email);

        // Em produção, salvar usuário no banco de dados
        var message = new { message = "Usuário registrado com sucesso. Faça login para obter o token." };

        return CreatedAtAction(nameof(Login), message);
    }
}
