using System.Data;
using Dapper;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.ValueObjects;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de Usuários usando Dapper
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Username, Email, PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt, RefreshToken, RefreshTokenExpiryTime
            FROM Users
            WHERE Id = @Id";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { id });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Username, Email, PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt, RefreshToken, RefreshTokenExpiryTime
            FROM Users
            WHERE Username = @Username";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { username });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Username, Email, PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt, RefreshToken, RefreshTokenExpiryTime
            FROM Users
            WHERE Email = @Email";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { email });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Username, Email, PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt, RefreshToken, RefreshTokenExpiryTime
            FROM Users
            ORDER BY CreatedAt DESC";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic);
    }

    public async Task<IEnumerable<User>> GetActiveAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Username, Email, PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt, RefreshToken, RefreshTokenExpiryTime
            FROM Users
            WHERE IsActive = 1
            ORDER BY CreatedAt DESC";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic);
    }

    public async Task AddAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO Users (Id, Username, Email, PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt, RefreshToken, RefreshTokenExpiryTime)
            VALUES (@Id, @Username, @Email, @PasswordHash, @FullName, @Role, @IsActive, @LastLoginAt, @CreatedAt, @UpdatedAt, @RefreshToken, @RefreshTokenExpiryTime)";

        await connection.ExecuteAsync(sql, new
        {
            user.Id,
            user.Username,
            Email = user.Email.Address,
            user.PasswordHash,
            user.FullName,
            Role = (int)user.Role,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt,
            user.UpdatedAt,
            user.RefreshToken,
            user.RefreshTokenExpiryTime
        });
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE Users
            SET Username = @Username,
                Email = @Email,
                PasswordHash = @PasswordHash,
                FullName = @FullName,
                Role = @Role,
                IsActive = @IsActive,
                LastLoginAt = @LastLoginAt,
                UpdatedAt = @UpdatedAt,
                RefreshToken = @RefreshToken,
                RefreshTokenExpiryTime = @RefreshTokenExpiryTime
            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new
        {
            user.Id,
            user.Username,
            Email = user.Email.Address,
            user.PasswordHash,
            user.FullName,
            Role = (int)user.Role,
            user.IsActive,
            user.LastLoginAt,
            user.UpdatedAt,
            user.RefreshToken,
            user.RefreshTokenExpiryTime
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = "DELETE FROM Users WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

        var count = await connection.QuerySingleAsync<int>(sql, new { username });

        return count > 0;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

        var count = await connection.QuerySingleAsync<int>(sql, new { email });

        return count > 0;
    }

    private static User MapFromDynamic(dynamic data)
    {
        return new User(
            (Guid)data.Id,
            (string)data.Username,
            new Email((string)data.Email),
            (string)data.PasswordHash,
            (string)data.FullName,
            (UserRole)(int)data.Role,
            (bool)data.IsActive,
            (DateTime?)data.LastLoginAt,
            (DateTime)data.CreatedAt,
            (DateTime?)data.UpdatedAt,
            (string?)data.RefreshToken,
            (DateTime?)data.RefreshTokenExpiryTime
        );
    }
}
