using Microsoft.AspNetCore.Http;

namespace AutonomousMarketingPlatform.Application.Services;

/// <summary>
/// Servicio para almacenamiento temporal y permanente de archivos.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Guarda un archivo temporalmente.
    /// </summary>
    Task<string> SaveTemporaryFileAsync(IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la URL de vista previa de un archivo.
    /// </summary>
    string GetPreviewUrl(string filePath, string contentType);

    /// <summary>
    /// Mueve un archivo de temporal a permanente.
    /// </summary>
    Task<string> MoveToPermanentAsync(string tempFilePath, Guid contentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un archivo temporal.
    /// </summary>
    Task DeleteTemporaryFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un archivo permanente.
    /// </summary>
    Task DeletePermanentFileAsync(string filePath, CancellationToken cancellationToken = default);
}

