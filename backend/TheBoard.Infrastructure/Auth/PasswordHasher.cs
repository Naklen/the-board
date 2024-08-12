using System.Security.Cryptography;
using TheBoard.Application.Auth;

namespace TheBoard.Infrastructure.Auth;

//thx https://stackoverflow.com/a/73125177
public class PasswordHasher : IPasswordHasher
{
    private const int _saltSize = 16; // 128 bits
    private const int _keySize = 32; // 256 bits
    private const int _iterations = 50000;
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

    private const char segmentDelimiter = ':';


    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            _iterations,
            _algorithm,
            _keySize
        );
        return string.Join(
            segmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            _iterations,
            _algorithm
        );
    }

    public bool Verify(string password, string hashedPassword)
    {
        string[] segments = hashedPassword.Split(segmentDelimiter);
        byte[] hash = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);
        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            algorithm,
            hash.Length
        );
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}
