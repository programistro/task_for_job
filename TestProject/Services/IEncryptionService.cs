namespace TestProject.Services;

public interface IEncryptionService
{
    string Decrypt(string encryptedTextBytesB64, string keyBytesB64);
}
