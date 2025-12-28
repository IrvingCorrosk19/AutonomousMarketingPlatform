using AutonomousMarketingPlatform.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Servicio de encriptación usando AES.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly ILogger<EncryptionService> _logger;

    public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
    {
        _logger = logger;
        
        // Obtener clave de encriptación desde configuración o generar una
        var encryptionKey = configuration["Encryption:Key"];
        if (string.IsNullOrEmpty(encryptionKey))
        {
            _logger.LogWarning("No se encontró Encryption:Key en configuración. Generando clave temporal (NO SEGURA PARA PRODUCCIÓN)");
            // En producción, esto DEBE venir de un secret manager
            encryptionKey = "TEMP_KEY_NOT_SECURE_CHANGE_IN_PRODUCTION_32_CHARS!!";
        }

        // La clave debe ser de 32 bytes para AES-256
        _key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0, 32));
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            
            // Escribir IV primero
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);
            
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al encriptar texto");
            throw;
        }
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return string.Empty;

        try
        {
            var fullCipher = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Leer IV (primeros 16 bytes)
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, 16);
            aes.IV = iv;

            // Leer datos encriptados (resto)
            var cipher = new byte[fullCipher.Length - 16];
            Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desencriptar texto");
            throw;
        }
    }
}

