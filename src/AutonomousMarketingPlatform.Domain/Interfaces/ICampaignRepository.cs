using AutonomousMarketingPlatform.Domain.Entities;

namespace AutonomousMarketingPlatform.Domain.Interfaces;

/// <summary>
/// Interfaz específica para el repositorio de Campaigns.
/// Extiende IRepository con métodos adicionales específicos de campañas.
/// </summary>
public interface ICampaignRepository : IRepository<Campaign>
{
    /// <summary>
    /// Obtiene todas las campañas activas de un tenant.
    /// </summary>
    Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una campaña con todos sus detalles (contenidos, memorias, etc.).
    /// </summary>
    Task<Campaign?> GetCampaignWithDetailsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
}

