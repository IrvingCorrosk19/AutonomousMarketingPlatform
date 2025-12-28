using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutonomousMarketingPlatform.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos principal de la aplicación.
/// Implementa filtrado automático por tenant para garantizar aislamiento de datos.
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly ITenantService? _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService? tenantService = null)
        : base(options)
    {
        _tenantService = tenantService;
    }

    // DbSets
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Consent> Consents { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<MarketingMemory> MarketingMemories { get; set; }
    public DbSet<AutomationState> AutomationStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Tenant
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Subdomain).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Subdomain).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(255);
        });

        // Configuración de User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Consent
        modelBuilder.Entity<Consent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.UserId, e.ConsentType });
            entity.Property(e => e.ConsentType).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Consents)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Campaign
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Campaigns)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Content
        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(1000);
            
            entity.HasOne(e => e.Campaign)
                .WithMany(c => c.Contents)
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de UserPreference
        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.UserId, e.PreferenceKey });
            entity.Property(e => e.PreferenceKey).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Preferences)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de MarketingMemory
        modelBuilder.Entity<MarketingMemory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MemoryType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Content).IsRequired();
            
            entity.HasOne(e => e.Campaign)
                .WithMany(c => c.MarketingMemories)
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de AutomationState
        modelBuilder.Entity<AutomationState>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AutomationType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Campaign)
                .WithMany()
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Aplicar índices para mejorar rendimiento en consultas multi-tenant
        ApplyTenantIndexes(modelBuilder);
    }

    /// <summary>
    /// Aplica índices en TenantId para todas las entidades que implementan ITenantEntity.
    /// Esto mejora significativamente el rendimiento de las consultas filtradas por tenant.
    /// </summary>
    private void ApplyTenantIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(e => e.TenantId);
        modelBuilder.Entity<Consent>().HasIndex(e => e.TenantId);
        modelBuilder.Entity<Campaign>().HasIndex(e => e.TenantId);
        modelBuilder.Entity<Content>().HasIndex(e => e.TenantId);
        modelBuilder.Entity<UserPreference>().HasIndex(e => e.TenantId);
        modelBuilder.Entity<MarketingMemory>().HasIndex(e => e.TenantId);
        modelBuilder.Entity<AutomationState>().HasIndex(e => e.TenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Solo asignar TenantId si TenantService está disponible (no en tiempo de diseño)
        if (_tenantService != null)
        {
            var tenantId = _tenantService.GetCurrentTenantId();

            // Asegurar que todas las entidades ITenantEntity tengan TenantId asignado
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Domain.Common.ITenantEntity && e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (entry.Entity is Domain.Common.ITenantEntity tenantEntity)
                {
                    if (tenantEntity.TenantId == Guid.Empty && tenantId.HasValue)
                    {
                        tenantEntity.TenantId = tenantId.Value;
                    }
                }
            }
        }

        // Actualizar UpdatedAt para entidades modificadas
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity && 
                       (e.State == EntityState.Modified || e.State == EntityState.Added));

        foreach (var entry in modifiedEntries)
        {
            if (entry.Entity is Domain.Common.BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    baseEntity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    baseEntity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

