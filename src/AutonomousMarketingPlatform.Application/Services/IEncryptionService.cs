namespace AutonomousMarketingPlatform.Application.Services;

/// <summary>
/// Servicio para encriptar/desencriptar datos sensibles como API keys.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encripta un valor.
    /// </summary>
    string Encrypt(string plainText);

    /// <summary>
    /// Desencripta un valor.
    /// </summary>
    string Decrypt(string encryptedText);
}

