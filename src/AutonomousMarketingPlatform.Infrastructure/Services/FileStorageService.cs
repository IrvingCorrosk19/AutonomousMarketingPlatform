using AutonomousMarketingPlatform.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de almacenamiento de archivos.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _webRootPath;
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _tempFolder;
    private readonly string _permanentFolder;

    public FileStorageService(string webRootPath, ILogger<FileStorageService> logger)
    {
        _webRootPath = webRootPath ?? throw new ArgumentNullException(nameof(webRootPath));
        _logger = logger;
        
        // Configurar rutas de almacenamiento
        _tempFolder = Path.Combine(_webRootPath, "uploads", "temp");
        _permanentFolder = Path.Combine(_webRootPath, "uploads", "permanent");

        // Crear directorios si no existen
        Directory.CreateDirectory(_tempFolder);
        Directory.CreateDirectory(_permanentFolder);
    }

    public async Task<string> SaveTemporaryFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            // Generar nombre único para el archivo temporal
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_tempFolder, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            _logger.LogInformation("Archivo temporal guardado: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar archivo temporal {FileName}", file.FileName);
            throw;
        }
    }

    public string GetPreviewUrl(string filePath, string contentType)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        // Si es un archivo temporal, generar URL relativa
        if (filePath.Contains("temp"))
        {
            var fileName = Path.GetFileName(filePath);
            return $"/uploads/temp/{fileName}";
        }

        // Si es permanente
        if (filePath.Contains("permanent"))
        {
            var fileName = Path.GetFileName(filePath);
            return $"/uploads/permanent/{fileName}";
        }

        return string.Empty;
    }

    public async Task<string> MoveToPermanentAsync(string tempFilePath, Guid contentId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(tempFilePath))
            {
                throw new FileNotFoundException($"Archivo temporal no encontrado: {tempFilePath}");
            }

            // Generar nombre permanente basado en contentId
            var extension = Path.GetExtension(tempFilePath);
            var permanentFileName = $"{contentId}{extension}";
            var permanentFilePath = Path.Combine(_permanentFolder, permanentFileName);

            // Mover archivo
            File.Move(tempFilePath, permanentFilePath, overwrite: true);

            _logger.LogInformation("Archivo movido a permanente: {FilePath}", permanentFilePath);
            return permanentFilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mover archivo a permanente {TempFilePath}", tempFilePath);
            throw;
        }
    }

    public async Task DeleteTemporaryFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Archivo temporal eliminado: {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al eliminar archivo temporal {FilePath}", filePath);
        }

        await Task.CompletedTask;
    }

    public async Task DeletePermanentFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Archivo permanente eliminado: {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al eliminar archivo permanente {FilePath}", filePath);
        }

        await Task.CompletedTask;
    }
}

