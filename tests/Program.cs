using System;
using System.Threading.Tasks;
using AutonomousMarketingPlatform.Tests;

namespace TestRunner;

class Program
{
    static async Task Main(string[] args)
    {
        var baseUrl = args.Length > 0 ? args[0] : "http://localhost:56610";
        Console.WriteLine($"Ejecutando pruebas contra: {baseUrl}\n");
        Console.WriteLine("Asegúrate de que la aplicación esté ejecutándose.\n");

        using var runner = new TestRunner(baseUrl);
        await runner.RunAllTestsAsync();
    }
}


