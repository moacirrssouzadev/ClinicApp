using System.Security.Cryptography;
using System.Text;
using ClinicApp.Domain.Security;

namespace ClinicApp.Infrastructure.Security;

/// <summary>
/// Implementação de hash de senha usando PBKDF2
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 20; // 160 bit
    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        byte[] salt;
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt = new byte[SaltSize]);
        }

        byte[] hash;
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            hash = pbkdf2.GetBytes(HashSize);
        }

        byte[] hashWithSalt = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashWithSalt, 0, SaltSize);
        Array.Copy(hash, 0, hashWithSalt, SaltSize, HashSize);

        return Convert.ToBase64String(hashWithSalt);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            byte[] hashWithSalt = Convert.FromBase64String(hash);

            // Verificar se o tamanho do hash é compatível (SaltSize + HashSize)
            if (hashWithSalt.Length != SaltSize + HashSize)
            {
                return false;
            }

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashWithSalt, 0, salt, 0, SaltSize);

            byte[] storedHash = new byte[HashSize];
            Array.Copy(hashWithSalt, SaltSize, storedHash, 0, HashSize);

            byte[] computedHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                computedHash = pbkdf2.GetBytes(HashSize);
            }

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch (FormatException)
        {
            // O hash não é uma string Base64 válida
            return false;
        }
    }
}
