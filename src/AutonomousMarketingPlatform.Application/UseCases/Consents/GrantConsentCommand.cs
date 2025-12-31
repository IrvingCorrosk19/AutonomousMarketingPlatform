using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GrantConsentCommandHandler(
        IRepository<Consent> consentRepository,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _consentRepository = consentRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ConsentDto> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el usuario existe
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new UnauthorizedAccessException($"Usuario con ID {request.UserId} no encontrado.");
        }
        
        // Si el TenantId no coincide, usar el TenantId del usuario en la base de datos
        // Esto puede pasar si hay un problema con los claims, pero el usuario existe
        var effectiveTenantId = user.TenantId != Guid.Empty ? user.TenantId : request.TenantId;

        // Obtener IP si no se proporcionó
        var ipAddress = request.IpAddress ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        // Buscar consentimiento existente
        var existingConsents = await _consentRepository.FindAsync(
            c => c.UserId == request.UserId && c.ConsentType == request.ConsentType,
            effectiveTenantId,
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
                TenantId = effectiveTenantId,
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

