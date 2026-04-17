using ClinicApp.Domain.Security;
using InfrastructurePasswordHasher = ClinicApp.Infrastructure.Security.PasswordHasher;

namespace ClinicApp.Infrastructure.Security;

/// <summary>
/// Adapter para compatibilidade com a interface do Application
/// </summary>
public class PasswordHasherAdapter : IPasswordHasher
{
    private readonly InfrastructurePasswordHasher _hasher;

    public PasswordHasherAdapter()
    {
        _hasher = new InfrastructurePasswordHasher();
    }

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return _hasher.VerifyPassword(password, hash);
    }
}
