using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;

namespace AutonomousMarketingPlatform.Application.UseCases.Consents;

/// <summary>
/// Comando para revocar un consentimiento.
/// </summary>
public class RevokeConsentCommand : IRequest<ConsentDto>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
}

/// <summary>
/// Handler para revocar consentimiento.
/// </summary>
public class RevokeConsentCommandHandler : IRequestHandler<RevokeConsentCommand, ConsentDto>
{
    private readonly IRepository<Consent> _consentRepository;
    private readonly IRepository<User> _userRepository;

    public RevokeConsentCommandHandler(
        IRepository<Consent> consentRepository,
        IRepository<User> userRepository)
    {
        _consentRepository = consentRepository;
        _userRepository = userRepository;
    }

    public async Task<ConsentDto> Handle(RevokeConsentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el usuario existe
        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o no pertenece al tenant.");
        }

        // Verificar que el consentimiento no es requerido
        var requiredConsentTypes = new[] { "AIGeneration", "DataProcessing" };
        if (requiredConsentTypes.Contains(request.ConsentType))
        {
            throw new InvalidOperationException($"El consentimiento '{request.ConsentType}' es requerido y no puede ser revocado.");
        }

        // Buscar consentimiento existente
        var existingConsents = await _consentRepository.FindAsync(
            c => c.UserId == request.UserId && c.ConsentType == request.ConsentType,
            request.TenantId,
            cancellationToken);

        var consent = existingConsents.FirstOrDefault();
        if (consent == null || !consent.IsGranted)
        {
            throw new InvalidOperationException("Consentimiento no encontrado o ya revocado.");
        }

        // Revocar consentimiento
        consent.IsGranted = false;
        consent.RevokedAt = DateTime.UtcNow;
        await _consentRepository.UpdateAsync(consent, cancellationToken);

        return new ConsentDto
        {
            Id = consent.Id,
            UserId = consent.UserId,
            ConsentType = consent.ConsentType,
            IsGranted = consent.IsGranted,
            GrantedAt = consent.GrantedAt,
            RevokedAt = consent.RevokedAt,
            ConsentVersion = consent.ConsentVersion
        };
    }
}

