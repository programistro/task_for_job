using System.Security.Cryptography;
using System.Text;

namespace TestProject.Services;

public class EncryptionService : IEncryptionService
{
    private const int AesBlockSizeInBytes = 16;

    private static readonly int[] ValidKeySizesInBytes = [16, 24, 32];

    private readonly ILogger<EncryptionService> _logger;

    public EncryptionService(ILogger<EncryptionService> logger)
    {
        _logger = logger;
    }

    public string Decrypt(string encryptedTextBytesB64, string keyBytesB64)
    {
        var key = Convert.FromBase64String(keyBytesB64);
        var cipherText = Convert.FromBase64String(encryptedTextBytesB64);

        if (Array.IndexOf(ValidKeySizesInBytes, key.Length) < 0)
        {
            throw new ArgumentException(
                $"AES key must be 16, 24 or 32 bytes long, but key_bytes_b64 decodes to {key.Length} bytes.",
                nameof(keyBytesB64));
        }

        if (cipherText.Length == 0 || cipherText.Length % AesBlockSizeInBytes != 0)
        {
            throw new CryptographicException(
                $"Ciphertext must decode to a positive multiple of the {AesBlockSizeInBytes}-byte AES block size, but it decoded to {cipherText.Length} bytes.");
        }

        try
        {
            return Encoding.UTF8.GetString(DecryptBlock(key, cipherText, PaddingMode.PKCS7));
        }
        catch (CryptographicException)
        {
            _logger.LogWarning("No valid PKCS7 padding, retrying with PaddingMode.None.");
            return Encoding.UTF8.GetString(DecryptBlock(key, cipherText, PaddingMode.None)).TrimEnd('\0');
        }
    }

    private static byte[] DecryptBlock(byte[] key, byte[] cipherText, PaddingMode paddingMode)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = paddingMode;
        aes.Key = key;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
    }
}
