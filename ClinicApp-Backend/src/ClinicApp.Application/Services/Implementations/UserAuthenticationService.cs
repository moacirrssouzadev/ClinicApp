using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Security;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de autenticação de usuários
/// </summary>
public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPatientRepository _patientRepository;
    private readonly IHealthProfessionalRepository _healthProfessionalRepository;

    public UserAuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IPatientRepository patientRepository,
        IHealthProfessionalRepository healthProfessionalRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _healthProfessionalRepository = healthProfessionalRepository ?? throw new ArgumentNullException(nameof(healthProfessionalRepository));
    }

    public async Task<LoginResponseDto> LoginAsync(LoginUserDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Usuário e senha são obrigatórios"
            };
        }

        // Tentar buscar por Username primeiro, se não encontrar, buscar por Email
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        
        if (user == null)
        {
            user = await _userRepository.GetByEmailAsync(loginDto.Username);
        }

        if (user == null || !user.IsActive)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Usuário ou senha inválidos"
            };
        }

        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Usuário ou senha inválidos"
            };
        }

        // Atualizar último login
        user.UpdateLastLogin();
        
        // Gerar tokens
        var token = _jwtTokenService.GenerateToken(user.Id, user.Username, (int)user.Role);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        Guid? patientId = null;
        Guid? professionalId = null;

        // Buscar ID do perfil correspondente
        if (user.Role == UserRole.Patient)
        {
            var patient = await _patientRepository.GetByEmailAsync(user.Email.Address);
            patientId = patient?.Id;
        }
        else if (user.Role == UserRole.HealthProfessional)
        {
            var professional = await _healthProfessionalRepository.GetByEmailAsync(user.Email.Address);
            professionalId = professional?.Id;
        }

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login realizado com sucesso",
            User = MapToUserDto(user),
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(60),
            PatientId = patientId,
            HealthProfessionalId = professionalId
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Refresh token é obrigatório"
            };
        }

        // Buscar usuário pelo refresh token
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Refresh token inválido ou expirado"
            };
        }

        // Gerar novos tokens
        var newToken = _jwtTokenService.GenerateToken(user.Id, user.Username, (int)user.Role);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        // Buscar IDs relacionados (mesma lógica do login)
        Guid? patientId = null;
        var patient = await _patientRepository.GetByEmailAsync(user.Email.Address);
        if (patient != null) patientId = patient.Id;

        Guid? professionalId = null;
        var professional = await _healthProfessionalRepository.GetByEmailAsync(user.Email.Address);
        if (professional != null) professionalId = professional.Id;

        return new LoginResponseDto
        {
            Success = true,
            User = MapToUserDto(user),
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(60),
            PatientId = patientId,
            HealthProfessionalId = professionalId
        };
    }

    public async Task<UserDto?> RegisterAsync(CreateUserDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Username) ||
            string.IsNullOrWhiteSpace(registerDto.Email) ||
            string.IsNullOrWhiteSpace(registerDto.Password) ||
            string.IsNullOrWhiteSpace(registerDto.FullName))
        {
            throw new ArgumentException("Todos os campos são obrigatórios");
        }

        // Verificar se usuário já existe
        if (await _userRepository.UserExistsAsync(registerDto.Username))
        {
            throw new InvalidOperationException($"Usuário '{registerDto.Username}' já existe");
        }

        // Verificar se email já existe
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException($"Email '{registerDto.Email}' já está registrado");
        }

        // Hash da senha
        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);

        // Criar novo usuário
        var user = new User(
            registerDto.Username,
            new Email(registerDto.Email),
            passwordHash,
            registerDto.FullName,
            (UserRole)registerDto.Role
        );

        // Salvar no repositório
        await _userRepository.AddAsync(user);

        return MapToUserDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<bool> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("Senhas não podem ser vazias");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Senha atual inválida");
        }

        var newPasswordHash = _passwordHasher.HashPassword(newPassword);
        user.UpdatePassword(newPasswordHash);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.FullName))
        {
            // Criar uma nova instância com os dados atualizados
            var updatedUser = new User(
                user.Id,
                user.Username,
                user.Email,
                user.PasswordHash,
                updateDto.FullName,
                updateDto.Role.HasValue ? (UserRole)updateDto.Role.Value : user.Role,
                updateDto.IsActive ?? user.IsActive,
                user.LastLoginAt,
                user.CreatedAt,
                DateTime.UtcNow
            );

            await _userRepository.UpdateAsync(updatedUser);
            return true;
        }

        return false;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.SetActive(false);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task LogoutAsync(Guid userId)
    {
        // Implementação básica - pode ser estendida para usar uma blacklist de tokens
        await Task.CompletedTask;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Address,
            FullName = user.FullName,
            Role = (int)user.Role,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
