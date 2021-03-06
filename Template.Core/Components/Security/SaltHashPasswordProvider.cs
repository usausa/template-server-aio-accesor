namespace Template.Components.Security;

using System.Security.Cryptography;
using System.Text;

public sealed class SaltHashPasswordProvider : IPasswordProvider
{
    private readonly SaltHashPasswordOptions options;

    public SaltHashPasswordProvider(SaltHashPasswordOptions options)
    {
        this.options = options;
    }

    public bool Match(string password, string hash)
    {
        var salt = hash[..options.SaltLength];

        using var algorithm = SHA256.Create();
        var bytes = algorithm.ComputeHash(Encoding.ASCII.GetBytes(salt + password));
        return salt + Convert.ToBase64String(bytes) == hash;
    }

    public string GenerateHash(string password)
    {
        var salt = GenerateSalt();

        using var algorithm = SHA256.Create();
        var bytes = algorithm.ComputeHash(Encoding.ASCII.GetBytes(salt + password));
        return salt + Convert.ToBase64String(bytes);
    }

    private string GenerateSalt()
    {
        var sb = new StringBuilder(options.SaltLength);

        for (var i = 0; i < options.SaltLength; i++)
        {
            var index = RandomNumberGenerator.GetInt32(options.SaltCharacters.Length);
            sb.Append(options.SaltCharacters[index]);
        }

        return sb.ToString();
    }
}
