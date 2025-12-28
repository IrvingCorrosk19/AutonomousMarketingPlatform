using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa un usuario del sistema.
/// Cada usuario pertenece a un tenant específico.
/// </summary>
public class User : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece el usuario.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Email del usuario (usado para login).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña del usuario.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario dentro del tenant (Admin, Manager, User).
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Indica si el usuario ha verificado su email.
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Token de verificación de email.
    /// </summary>
    public string? EmailVerificationToken { get; set; }

    /// <summary>
    /// Fecha del último inicio de sesión.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<Consent> Consents { get; set; } = new List<Consent>();
    public virtual ICollection<UserPreference> Preferences { get; set; } = new List<UserPreference>();
}

