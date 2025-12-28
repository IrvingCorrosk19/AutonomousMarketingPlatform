using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;

namespace AutonomousMarketingPlatform.Application.UseCases.Consents;

/// <summary>
/// Query para validar si un usuario tiene un consentimiento específico otorgado.
/// </summary>
public class ValidateConsentQuery : IRequest<bool>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
}

/// <summary>
/// Handler para validar consentimiento.
/// </summary>
public class ValidateConsentQueryHandler : IRequestHandler<ValidateConsentQuery, bool>
{
    private readonly IRepository<Domain.Entities.Consent> _consentRepository;

    public ValidateConsentQueryHandler(IRepository<Domain.Entities.Consent> consentRepository)
    {
        _consentRepository = consentRepository;
    }

    public async Task<bool> Handle(ValidateConsentQuery request, CancellationToken cancellationToken)
    {
        var consents = await _consentRepository.FindAsync(
            c => c.UserId == request.UserId && 
                 c.ConsentType == request.ConsentType && 
                 c.IsGranted,
            request.TenantId,
            cancellationToken);

        var consent = consents.FirstOrDefault();
        return consent != null && consent.IsGranted && consent.RevokedAt == null;
    }
}

