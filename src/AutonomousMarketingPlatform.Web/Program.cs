using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Infrastructure.Data;
using AutonomousMarketingPlatform.Infrastructure.Repositories;
using AutonomousMarketingPlatform.Infrastructure.Services;
using AutonomousMarketingPlatform.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configurar Entity Framework Core con PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Registrar servicios de infraestructura primero (para evitar dependencia circular)
builder.Services.AddHttpContextAccessor();

// Agregar DbContextFactory para TenantService (sin dependencia circular)
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar DbContext con acceso a IServiceProvider
// Usamos AddDbContextFactory para evitar dependencia circular en el registro
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registrar ApplicationDbContext manualmente para pasar IServiceProvider
builder.Services.AddScoped<ApplicationDbContext>(serviceProvider =>
{
    var factory = serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    var dbContext = factory.CreateDbContext();
    
    // Inyectar IServiceProvider e IHttpContextAccessor usando reflection o simplemente usar el factory
    // Para evitar la dependencia circular, usamos el factory directamente
    var httpContextAccessor = serviceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
    
    // Crear nuevo contexto con los servicios necesarios
    var options = serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
    return new ApplicationDbContext(options, httpContextAccessor, serviceProvider);
});

// Registrar TenantService (usa IDbContextFactory, no ApplicationDbContext directamente)
builder.Services.AddScoped<ITenantService, TenantService>();

// Registrar repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

// Registrar MediatR para CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("AutonomousMarketingPlatform.Application")));

// Registrar servicios de consentimiento
builder.Services.AddScoped<IConsentValidationService, ConsentValidationService>();

// Registrar servicios de almacenamiento de archivos
builder.Services.AddScoped<IFileStorageService>(serviceProvider =>
{
    var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var logger = serviceProvider.GetRequiredService<ILogger<FileStorageService>>();
    return new FileStorageService(environment.WebRootPath ?? throw new InvalidOperationException("WebRootPath no configurado"), logger);
});

// Configurar CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware de validación de consentimientos (antes de autorización)
app.UseConsentValidation();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

