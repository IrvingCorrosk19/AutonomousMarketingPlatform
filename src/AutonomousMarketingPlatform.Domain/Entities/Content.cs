using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa contenido cargado por el usuario o generado por IA.
/// Puede ser imágenes, videos, texto, etc.
/// </summary>
public class Content : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece el contenido.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador de la campaña asociada (opcional).
    /// </summary>
    public Guid? CampaignId { get; set; }

    /// <summary>
    /// Tipo de contenido (Image, Video, Text, ReferenceImage, ReferenceVideo).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// URL o ruta del archivo de contenido.
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Nombre original del archivo.
    /// </summary>
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Tamaño del archivo en bytes.
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// MIME type del contenido.
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Indica si el contenido fue generado por IA o cargado por el usuario.
    /// </summary>
    public bool IsAiGenerated { get; set; }

    /// <summary>
    /// Descripción o metadata del contenido.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tags asociados al contenido para búsqueda y organización.
    /// </summary>
    public string? Tags { get; set; }

    // Navigation properties
    public virtual Campaign? Campaign { get; set; }
}

