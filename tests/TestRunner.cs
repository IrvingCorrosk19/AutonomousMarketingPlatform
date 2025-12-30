using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AutonomousMarketingPlatform.Tests;

/// <summary>
/// Script de pruebas funcionales para ejecutar manualmente.
/// Ejecutar la aplicación primero y luego este script.
/// </summary>
public class TestRunner
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private int _testsPassed = 0;
    private int _testsFailed = 0;
    private readonly List<string> _errors = new();

    public TestRunner(string baseUrl = "http://localhost:56610")
    {
        _baseUrl = baseUrl;
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/html"));
    }

    public async Task RunAllTestsAsync()
    {
        Console.WriteLine("=== INICIANDO PRUEBAS FUNCIONALES ===\n");

        await TestLoginPage();
        await TestLoginInvalidCredentials();
        await TestLoginValidCredentials();
        await TestDashboardAccess();
        await TestCampaignsList();
        await TestCampaignCreateForm();
        await TestCampaignCreateValidation();

        PrintSummary();
    }

    private async Task TestLoginPage()
    {
        Console.Write("Test 1: Página de Login (GET) ... ");
        try
        {
            var response = await _client.GetAsync("/Account/Login");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("Login", StringComparison.OrdinalIgnoreCase))
                {
                    Pass();
                    return;
                }
            }
            Fail("No se pudo cargar la página de login");
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private async Task TestLoginInvalidCredentials()
    {
        Console.Write("Test 2: Login con credenciales inválidas ... ");
        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("Email", "invalid@test.com"),
                new("Password", "WrongPassword123!")
            };
            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Account/Login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (responseContent.Contains("incorrecto", StringComparison.OrdinalIgnoreCase) ||
                responseContent.Contains("inválido", StringComparison.OrdinalIgnoreCase) ||
                responseContent.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                Pass();
            }
            else
            {
                Fail("No se mostró mensaje de error apropiado");
            }
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private async Task TestLoginValidCredentials()
    {
        Console.Write("Test 3: Login con credenciales válidas ... ");
        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("Email", "admin@test.com"),
                new("Password", "Admin123!")
            };
            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Account/Login", content);
            
            // Guardar cookies para siguientes requests
            if (response.Headers.Contains("Set-Cookie"))
            {
                var cookies = response.Headers.GetValues("Set-Cookie");
                foreach (var cookie in cookies)
                {
                    _client.DefaultRequestHeaders.Add("Cookie", cookie);
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Redirect ||
                response.IsSuccessStatusCode)
            {
                var location = response.Headers.Location?.ToString() ?? "";
                if (location.Contains("Home") || location.Contains("Dashboard") || 
                    response.RequestMessage?.RequestUri?.ToString().Contains("Home") == true)
                {
                    Pass();
                    return;
                }
            }
            Fail("No se redirigió correctamente después del login");
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private async Task TestDashboardAccess()
    {
        Console.Write("Test 4: Acceso al Dashboard ... ");
        try
        {
            var response = await _client.GetAsync("/Home");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("Dashboard", StringComparison.OrdinalIgnoreCase) ||
                    content.Contains("Campaña", StringComparison.OrdinalIgnoreCase))
                {
                    Pass();
                    return;
                }
            }
            Fail("No se pudo acceder al dashboard");
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private async Task TestCampaignsList()
    {
        Console.Write("Test 5: Lista de Campañas ... ");
        try
        {
            var response = await _client.GetAsync("/Campaigns");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("Campaña", StringComparison.OrdinalIgnoreCase))
                {
                    Pass();
                    return;
                }
            }
            Fail("No se pudo cargar la lista de campañas");
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private async Task TestCampaignCreateForm()
    {
        Console.Write("Test 6: Formulario de Crear Campaña ... ");
        try
        {
            var response = await _client.GetAsync("/Campaigns/Create");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("Crear", StringComparison.OrdinalIgnoreCase) ||
                    content.Contains("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    Pass();
                    return;
                }
            }
            Fail("No se pudo cargar el formulario de crear campaña");
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private async Task TestCampaignCreateValidation()
    {
        Console.Write("Test 7: Validación de Crear Campaña (nombre vacío) ... ");
        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("Name", ""),
                new("Status", "Draft")
            };
            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Campaigns/Create", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (responseContent.Contains("obligatorio", StringComparison.OrdinalIgnoreCase) ||
                responseContent.Contains("requerido", StringComparison.OrdinalIgnoreCase) ||
                responseContent.Contains("Name", StringComparison.OrdinalIgnoreCase))
            {
                Pass();
            }
            else
            {
                Fail("No se mostró error de validación para nombre vacío");
            }
        }
        catch (Exception ex)
        {
            Fail($"Error: {ex.Message}");
        }
    }

    private void Pass()
    {
        _testsPassed++;
        Console.WriteLine("✓ PASSED");
    }

    private void Fail(string message)
    {
        _testsFailed++;
        _errors.Add(message);
        Console.WriteLine($"✗ FAILED: {message}");
    }

    private void PrintSummary()
    {
        Console.WriteLine("\n=== RESUMEN DE PRUEBAS ===");
        Console.WriteLine($"Total: {_testsPassed + _testsFailed}");
        Console.WriteLine($"Pasadas: {_testsPassed}");
        Console.WriteLine($"Fallidas: {_testsFailed}");
        
        if (_errors.Any())
        {
            Console.WriteLine("\nErrores encontrados:");
            foreach (var error in _errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}


