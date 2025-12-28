using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AutonomousMarketingPlatform.Application.UseCases.Consents;

/// <summary>
/// Comando para otorgar un consentimiento.
/// </summary>
public class GrantConsentCommand : IRequest<ConsentDto>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
    public string? ConsentVersion { get; set; }
    public string? IpAddress { get; set; }
}

/// <summary>
/// Handler para otorgar consentimiento.
/// </summary>
public class GrantConsentCommandHandler : IRequestHandler<GrantConsentCommand, ConsentDto>
{
    private readonly IRepository<Consent> _consentRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GrantConsentCommandHandler(
        IRepository<Consent> consentRepository,
        IRepository<User> userRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _consentRepository = consentRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ConsentDto> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el usuario existe y pertenece al tenant
        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o no pertenece al tenant.");
        }

        // Obtener IP si no se proporcionó
        var ipAddress = request.IpAddress ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        // Buscar consentimiento existente
        var existingConsents = await _consentRepository.FindAsync(
            c => c.UserId == request.UserId && c.ConsentType == request.ConsentType,
            request.TenantId,
            cancellationToken);

        var existingConsent = existingConsents.FirstOrDefault();

        Consent consent;
        if (existingConsent != null)
        {
            // Actualizar consentimiento existente
            existingConsent.IsGranted = true;
            existingConsent.GrantedAt = DateTime.UtcNow;
            existingConsent.RevokedAt = null;
            existingConsent.ConsentVersion = request.ConsentVersion ?? existingConsent.ConsentVersion;
            existingConsent.IpAddress = ipAddress;
            await _consentRepository.UpdateAsync(existingConsent, cancellationToken);
            consent = existingConsent;
        }
        else
        {
            // Crear nuevo consentimiento
            consent = new Consent
            {
                TenantId = request.TenantId,
                UserId = request.UserId,
                ConsentType = request.ConsentType,
                IsGranted = true,
                GrantedAt = DateTime.UtcNow,
                ConsentVersion = request.ConsentVersion ?? "1.0",
                IpAddress = ipAddress
            };
            await _consentRepository.AddAsync(consent, cancellationToken);
        }

        return new ConsentDto
        {
            Id = consent.Id,
            UserId = consent.UserId,
            ConsentType = consent.ConsentType,
            IsGranted = consent.IsGranted,
            GrantedAt = consent.GrantedAt,
            ConsentVersion = consent.ConsentVersion
        };
    }
}

