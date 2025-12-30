using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ContentEntity = AutonomousMarketingPlatform.Domain.Entities.Content;

namespace AutonomousMarketingPlatform.Application.UseCases.Content;

/// <summary>
/// Query para listar contenido del tenant.
/// </summary>
public class ListContentQuery : IRequest<List<ContentListItemDto>>
{
    public Guid TenantId { get; set; }
    public Guid? CampaignId { get; set; }
    public string? ContentType { get; set; } // Image, Video
    public bool? IsAiGenerated { get; set; }
}

/// <summary>
/// Handler para listar contenido.
/// </summary>
public class ListContentQueryHandler : IRequestHandler<ListContentQuery, List<ContentListItemDto>>
{
    private readonly IRepository<ContentEntity> _contentRepository;

    public ListContentQueryHandler(IRepository<ContentEntity> contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task<List<ContentListItemDto>> Handle(ListContentQuery request, CancellationToken cancellationToken)
    {
        var allContent = await _contentRepository.GetAllAsync(request.TenantId, cancellationToken);
        var contentList = allContent.ToList();

        // Aplicar filtros
        if (request.CampaignId.HasValue)
        {
            contentList = contentList.Where(c => c.CampaignId == request.CampaignId).ToList();
        }

        if (!string.IsNullOrEmpty(request.ContentType))
        {
            contentList = contentList.Where(c => c.ContentType == request.ContentType).ToList();
        }

        if (request.IsAiGenerated.HasValue)
        {
            contentList = contentList.Where(c => c.IsAiGenerated == request.IsAiGenerated.Value).ToList();
        }

        return contentList
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ContentListItemDto
            {
                Id = c.Id,
                ContentType = c.ContentType,
                Description = c.Description,
                FileUrl = c.FileUrl,
                OriginalFileName = c.OriginalFileName,
                FileSize = c.FileSize,
                MimeType = c.MimeType,
                IsAiGenerated = c.IsAiGenerated,
                Tags = c.Tags,
                CampaignId = c.CampaignId,
                CreatedAt = c.CreatedAt
            })
            .ToList();
    }
}

