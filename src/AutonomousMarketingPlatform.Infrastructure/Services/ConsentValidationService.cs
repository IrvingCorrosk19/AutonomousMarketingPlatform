using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Domain.Interfaces;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de validación de consentimientos.
/// </summary>
public class ConsentValidationService : IConsentValidationService
{
    private readonly IRepository<Domain.Entities.Consent> _consentRepository;

    public ConsentValidationService(IRepository<Domain.Entities.Consent> consentRepository)
    {
        _consentRepository = consentRepository;
    }

    public async Task<bool> ValidateConsentAsync(Guid userId, Guid tenantId, string consentType, CancellationToken cancellationToken = default)
    {
        var consents = await _consentRepository.FindAsync(
            c => c.UserId == userId && 
                 c.ConsentType == consentType && 
                 c.IsGranted &&
                 c.RevokedAt == null,
            tenantId,
            cancellationToken);

        return consents.Any();
    }

    public async Task<bool> ValidateConsentsAsync(Guid userId, Guid tenantId, IEnumerable<string> consentTypes, CancellationToken cancellationToken = default)
    {
        var allConsents = await _consentRepository.FindAsync(
            c => c.UserId == userId && 
                 c.IsGranted &&
                 c.RevokedAt == null,
            tenantId,
            cancellationToken);

        var grantedTypes = allConsents.Select(c => c.ConsentType).ToList();
        return consentTypes.All(ct => grantedTypes.Contains(ct));
    }

    public async Task<List<string>> GetMissingConsentsAsync(Guid userId, Guid tenantId, IEnumerable<string> requiredConsentTypes, CancellationToken cancellationToken = default)
    {
        var allConsents = await _consentRepository.FindAsync(
            c => c.UserId == userId && 
                 c.IsGranted &&
                 c.RevokedAt == null,
            tenantId,
            cancellationToken);

        var grantedTypes = allConsents.Select(c => c.ConsentType).ToHashSet();
        return requiredConsentTypes.Where(ct => !grantedTypes.Contains(ct)).ToList();
    }
}

