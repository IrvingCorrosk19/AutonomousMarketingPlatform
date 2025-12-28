namespace AutonomousMarketingPlatform.Application.DTOs;

/// <summary>
/// DTO para información de archivo cargado.
/// </summary>
public class FileUploadDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty; // Image, Video, etc.
    public string TempFilePath { get; set; } = string.Empty;
    public string? PreviewUrl { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid? CampaignId { get; set; }
}

/// <summary>
/// DTO para respuesta de carga múltiple.
/// </summary>
public class MultipleFileUploadResponseDto
{
    public List<FileUploadDto> SuccessfullyUploaded { get; set; } = new();
    public List<FileUploadErrorDto> Errors { get; set; } = new();
    public int TotalFiles { get; set; }
    public int SuccessfulUploads { get; set; }
    public int FailedUploads { get; set; }
}

/// <summary>
/// DTO para errores de carga.
/// </summary>
public class FileUploadErrorDto
{
    public string FileName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}

/// <summary>
/// DTO para validación de archivo antes de cargar.
/// </summary>
public class FileValidationDto
{
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

