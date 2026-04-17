using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services;

namespace ClinicApp.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários e autenticação
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserAuthenticationService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserAuthenticationService authService, ILogger<UsersController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Realiza login de um usuário
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginUserDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("Tentativa de login para usuário: {Username}", loginDto.Username);

        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
        {
            _logger.LogWarning("Login falhou para: {Username}", loginDto.Username);
            return Unauthorized(result);
        }

        _logger.LogInformation("Login bem-sucedido para: {Username}", loginDto.Username);
        return Ok(result);
    }

    /// <summary>
    /// Renova o token de acesso usando um refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("Tentativa de renovação de token");

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (!result.Success)
        {
            _logger.LogWarning("Falha ao renovar token: {Message}", result.Message);
            return Unauthorized(result);
        }

        _logger.LogInformation("Token renovado com sucesso para usuário: {Username}", result.User?.Username);
        return Ok(result);
    }

    /// <summary>
    /// Obtém todos os usuários
    /// </summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        _logger.LogInformation("Obtendo todos os usuários");
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _logger.LogInformation("Registrando novo usuário: {Username}", registerDto.Username);

            var user = await _authService.RegisterAsync(registerDto);

            if (user == null)
                return BadRequest("Erro ao registrar usuário");

            _logger.LogInformation("Usuário registrado com sucesso: {Username}", registerDto.Username);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao registrar usuário: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Erro de validação: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtém informações do usuário logado
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Obtém informações de um usuário por ID
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _authService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Atualiza informações do usuário
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateDto)
    {
        var userIdClaim = User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        // Usuário só pode atualizar seus próprios dados (ou admin)
        var rolesClaim = User.FindAll("role");
        bool isAdmin = rolesClaim.Any(r => r.Value == "0");

        if (userId != id && !isAdmin)
            return Unauthorized();

        _logger.LogInformation("Atualizando usuário: {UserId}", id);

        var success = await _authService.UpdateUserAsync(id, updateDto);

        if (!success)
            return NotFound();

        var user = await _authService.GetUserByIdAsync(id);
        return Ok(user);
    }

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    [Authorize]
    [HttpPost("{id}/change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        var userIdClaim = User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        // Usuário só pode alterar sua própria senha
        if (userId != id)
            return Unauthorized();

        try
        {
            _logger.LogInformation("Alterando senha do usuário: {UserId}", id);

            var success = await _authService.UpdatePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!success)
                return NotFound();

            return Ok(new { message = "Senha alterada com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao alterar senha: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Desativa um usuário
    /// </summary>
    [Authorize]
    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeactivateUser(Guid id)
    {
        var userIdClaim = User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        // Usuário pode desativar sua própria conta
        if (userId != id)
            return Unauthorized();

        _logger.LogInformation("Desativando usuário: {UserId}", id);

        var success = await _authService.DeactivateUserAsync(id);

        if (!success)
            return NotFound();

        return Ok(new { message = "Usuário desativado com sucesso" });
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout()
    {
        var userIdClaim = User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        _logger.LogInformation("Logout do usuário: {UserId}", userId);

        await _authService.LogoutAsync(userId);

        return Ok(new { message = "Logout realizado com sucesso" });
    }
}

/// <summary>
/// Request para renovação de token
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO para alteração de senha
/// </summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
