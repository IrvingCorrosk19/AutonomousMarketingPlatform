namespace AutonomousMarketingPlatform.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se puede resolver el tenant del request.
/// </summary>
public class TenantNotResolvedException : Exception
{
    public TenantNotResolvedException()
        : base("No se pudo resolver el tenant del request. Verifique que existe un tenant por defecto o proporcione un subdominio válido.")
    {
    }

    public TenantNotResolvedException(string message)
        : base(message)
    {
    }

    public TenantNotResolvedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

