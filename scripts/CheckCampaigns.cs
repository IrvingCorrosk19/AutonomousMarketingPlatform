using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutonomousMarketingPlatform.Infrastructure.Data;

namespace Scripts;

class Program
{
    static async Task Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=AutonomousMarketingPlatform;Username=postgres;Password=Panama2020$");

        using var context = new ApplicationDbContext(optionsBuilder.Options);

        var campaigns = await context.Campaigns
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.TenantId,
                c.Status,
                c.IsActive,
                c.CreatedAt
            })
            .ToListAsync();

        Console.WriteLine($"Total de campañas encontradas: {campaigns.Count}");
        Console.WriteLine();

        if (campaigns.Any())
        {
            Console.WriteLine("Campañas en la base de datos:");
            Console.WriteLine(new string('-', 100));
            foreach (var campaign in campaigns)
            {
                Console.WriteLine($"ID: {campaign.Id}");
                Console.WriteLine($"Nombre: {campaign.Name}");
                Console.WriteLine($"TenantId: {campaign.TenantId}");
                Console.WriteLine($"Estado: {campaign.Status}");
                Console.WriteLine($"Activa: {campaign.IsActive}");
                Console.WriteLine($"Creada: {campaign.CreatedAt}");
                Console.WriteLine(new string('-', 100));
            }
        }
        else
        {
            Console.WriteLine("No se encontraron campañas en la base de datos.");
        }

        // También verificar usuarios y sus TenantIds
        var users = await context.Users
            .Select(u => new { u.Id, u.Email, u.TenantId })
            .ToListAsync();

        Console.WriteLine();
        Console.WriteLine($"Total de usuarios: {users.Count}");
        foreach (var user in users)
        {
            Console.WriteLine($"Usuario: {user.Email}, TenantId: {user.TenantId}");
        }
    }
}

