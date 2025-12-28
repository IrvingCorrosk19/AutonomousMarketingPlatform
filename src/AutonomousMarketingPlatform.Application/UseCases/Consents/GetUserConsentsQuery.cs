using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;

namespace AutonomousMarketingPlatform.Application.UseCases.Consents;

/// <summary>
/// Query para obtener todos los consentimientos de un usuario.
/// </summary>
public class GetUserConsentsQuery : IRequest<UserConsentsDto>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
}

/// <summary>
/// Handler para obtener consentimientos del usuario.
/// </summary>
public class GetUserConsentsQueryHandler : IRequestHandler<GetUserConsentsQuery, UserConsentsDto>
{
    private readonly IRepository<Domain.Entities.Consent> _consentRepository;
    private readonly IRepository<Domain.Entities.User> _userRepository;

    public GetUserConsentsQueryHandler(
        IRepository<Domain.Entities.Consent> consentRepository,
        IRepository<Domain.Entities.User> userRepository)
    {
        _consentRepository = consentRepository;
        _userRepository = userRepository;
    }

    public async Task<UserConsentsDto> Handle(GetUserConsentsQuery request, CancellationToken cancellationToken)
    {
        // Verificar que el usuario existe y pertenece al tenant
        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o no pertenece al tenant.");
        }

        // Obtener todos los consentimientos del usuario
        var consents = await _consentRepository.FindAsync(
            c => c.UserId == request.UserId,
            request.TenantId,
            cancellationToken);

        // Definir tipos de consentimiento requeridos
        var requiredConsentTypes = new[] { "AIGeneration", "DataProcessing" };
        var allConsentTypes = new[]
        {
            new { Type = "AIGeneration", DisplayName = "Generación de Contenido con IA", Description = "Permite al sistema generar contenido publicitario usando inteligencia artificial.", Required = true },
            new { Type = "DataProcessing", DisplayName = "Procesamiento de Datos", Description = "Permite procesar y analizar datos para mejorar el marketing.", Required = true },
            new { Type = "AutoPublishing", DisplayName = "Publicación Automática", Description = "Permite publicar contenido automáticamente en redes sociales.", Required = false },
            new { Type = "Analytics", DisplayName = "Análisis y Métricas", Description = "Permite recopilar y analizar métricas de rendimiento.", Required = false }
        };

        var consentDtos = allConsentTypes.Select(ct =>
        {
            var existingConsent = consents.FirstOrDefault(c => c.ConsentType == ct.Type);
            return new ConsentDto
            {
                Id = existingConsent?.Id ?? Guid.Empty,
                UserId = request.UserId,
                ConsentType = ct.Type,
                ConsentTypeDisplayName = ct.DisplayName,
                Description = ct.Description,
                IsGranted = existingConsent?.IsGranted ?? false,
                GrantedAt = existingConsent?.GrantedAt,
                RevokedAt = existingConsent?.RevokedAt,
                ConsentVersion = existingConsent?.ConsentVersion,
                IsRequired = ct.Required,
                CanRevoke = existingConsent != null && existingConsent.IsGranted && !ct.Required
            };
        }).ToList();

        var missingRequired = consentDtos
            .Where(c => c.IsRequired && !c.IsGranted)
            .Select(c => c.ConsentTypeDisplayName)
            .ToList();

        return new UserConsentsDto
        {
            UserId = request.UserId,
            Consents = consentDtos,
            AllRequiredConsentsGranted = !missingRequired.Any(),
            MissingRequiredConsents = missingRequired
        };
    }
}

