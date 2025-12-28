using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ContentEntity = AutonomousMarketingPlatform.Domain.Entities.Content;

namespace AutonomousMarketingPlatform.Application.UseCases.Content;

/// <summary>
/// Comando para cargar múltiples archivos (imágenes/videos).
/// </summary>
public class UploadFilesCommand : IRequest<MultipleFileUploadResponseDto>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public List<IFormFile> Files { get; set; } = new();
    public Guid? CampaignId { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
}

/// <summary>
/// Handler para cargar archivos.
/// </summary>
public class UploadFilesCommandHandler : IRequestHandler<UploadFilesCommand, MultipleFileUploadResponseDto>
{
    private readonly IRepository<ContentEntity> _contentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<UploadFilesCommandHandler> _logger;

    // Configuración de validación
    private const long MaxFileSize = 100 * 1024 * 1024; // 100 MB
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
    private static readonly string[] AllowedVideoTypes = { "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/webm" };

    public UploadFilesCommandHandler(
        IRepository<ContentEntity> contentRepository,
        IFileStorageService fileStorageService,
        ILogger<UploadFilesCommandHandler> logger)
    {
        _contentRepository = contentRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<MultipleFileUploadResponseDto> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
    {
        var response = new MultipleFileUploadResponseDto
        {
            TotalFiles = request.Files.Count
        };

        foreach (var file in request.Files)
        {
            try
            {
                // Validar archivo
                var validation = ValidateFile(file);
                if (!validation.IsValid)
                {
                    response.Errors.Add(new FileUploadErrorDto
                    {
                        FileName = file.FileName,
                        ErrorMessage = string.Join(", ", validation.ValidationErrors),
                        ErrorCode = "VALIDATION_ERROR"
                    });
                    response.FailedUploads++;
                    continue;
                }

                // Determinar tipo de contenido
                var contentType = DetermineContentType(file.ContentType);

                // Guardar archivo temporalmente
                var tempFilePath = await _fileStorageService.SaveTemporaryFileAsync(file, cancellationToken);

                // Crear registro en base de datos
                var content = new ContentEntity
                {
                    TenantId = request.TenantId,
                    CampaignId = request.CampaignId,
                    ContentType = contentType,
                    FileUrl = tempFilePath,
                    OriginalFileName = file.FileName,
                    FileSize = file.Length,
                    MimeType = file.ContentType,
                    IsAiGenerated = false,
                    Description = request.Description,
                    Tags = request.Tags
                };

                await _contentRepository.AddAsync(content, cancellationToken);

                // Generar URL de vista previa
                var previewUrl = _fileStorageService.GetPreviewUrl(tempFilePath, contentType);

                response.SuccessfullyUploaded.Add(new FileUploadDto
                {
                    Id = content.Id,
                    FileName = file.FileName,
                    OriginalFileName = file.FileName,
                    FileSize = file.Length,
                    MimeType = file.ContentType,
                    ContentType = contentType,
                    TempFilePath = tempFilePath,
                    PreviewUrl = previewUrl,
                    UploadedAt = DateTime.UtcNow,
                    CampaignId = request.CampaignId
                });

                response.SuccessfulUploads++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar archivo {FileName}", file.FileName);
                response.Errors.Add(new FileUploadErrorDto
                {
                    FileName = file.FileName,
                    ErrorMessage = "Error al procesar el archivo. Por favor, intente nuevamente.",
                    ErrorCode = "UPLOAD_ERROR"
                });
                response.FailedUploads++;
            }
        }

        return response;
    }

    private FileValidationDto ValidateFile(IFormFile file)
    {
        var validation = new FileValidationDto
        {
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            IsValid = true
        };

        // Validar tamaño
        if (file.Length == 0)
        {
            validation.IsValid = false;
            validation.ValidationErrors.Add("El archivo está vacío.");
        }

        if (file.Length > MaxFileSize)
        {
            validation.IsValid = false;
            validation.ValidationErrors.Add($"El archivo excede el tamaño máximo permitido ({MaxFileSize / (1024 * 1024)} MB).");
        }

        // Validar tipo MIME
        var allowedTypes = AllowedImageTypes.Concat(AllowedVideoTypes).ToArray();
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            validation.IsValid = false;
            validation.ValidationErrors.Add($"Tipo de archivo no permitido. Tipos permitidos: imágenes (JPEG, PNG, GIF, WEBP) y videos (MP4, MPEG, MOV, AVI, WEBM).");
        }

        // Validar extensión del archivo
        var extension = Path.GetExtension(file.FileName).ToLower();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".mpeg", ".mov", ".avi", ".webm" };
        if (!allowedExtensions.Contains(extension))
        {
            validation.IsValid = false;
            validation.ValidationErrors.Add("Extensión de archivo no permitida.");
        }

        return validation;
    }

    private string DetermineContentType(string mimeType)
    {
        if (AllowedImageTypes.Contains(mimeType.ToLower()))
            return "Image";
        
        if (AllowedVideoTypes.Contains(mimeType.ToLower()))
            return "Video";
        
        return "Unknown";
    }
}

