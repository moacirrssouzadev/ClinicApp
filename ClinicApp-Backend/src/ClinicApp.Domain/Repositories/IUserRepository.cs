using ClinicApp.Domain.Entities;

namespace ClinicApp.Domain.Repositories;

/// <summary>
/// Interface para o repositório de Usuários
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetActiveAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> UserExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
