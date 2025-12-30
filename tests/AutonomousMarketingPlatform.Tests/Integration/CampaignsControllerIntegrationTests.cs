using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AutonomousMarketingPlatform.Tests.Integration;

/// <summary>
/// Pruebas de integración para CampaignsController.
/// Requiere autenticación previa.
/// </summary>
public class CampaignsControllerIntegrationTests : IClassFixture<AuthenticatedWebApplicationFactoryFixture>
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public CampaignsControllerIntegrationTests(AuthenticatedWebApplicationFactoryFixture fixture)
    {
        _client = fixture.Client;
        _baseUrl = fixture.BaseUrl;
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Index_ShouldReturnCampaignsList()
    {
        // Act
        var response = await _client.GetAsync("/Campaigns");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Campaña", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Create_Get_ShouldReturnCreateForm()
    {
        // Act
        var response = await _client.GetAsync("/Campaigns/Create");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Crear", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Create_Post_WithValidData_ShouldCreateCampaign()
    {
        // Arrange
        var campaignData = new
        {
            Name = "Campaña de Prueba",
            Description = "Descripción de prueba",
            Status = "Draft",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Budget = 50000
        };

        var formData = new List<KeyValuePair<string, string>>
        {
            new("Name", campaignData.Name),
            new("Description", campaignData.Description),
            new("Status", campaignData.Status),
            new("StartDate", campaignData.StartDate.ToString("yyyy-MM-dd")),
            new("EndDate", campaignData.EndDate.ToString("yyyy-MM-dd")),
            new("Budget", campaignData.Budget.ToString())
        };

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Campaigns/Create", formContent);

        // Assert
        // Debería redirigir a Details o Index
        Assert.True(response.StatusCode == HttpStatusCode.Redirect || 
                    response.StatusCode == HttpStatusCode.OK);
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Create_Post_WithInvalidData_ShouldReturnValidationErrors()
    {
        // Arrange - Nombre vacío
        var formData = new List<KeyValuePair<string, string>>
        {
            new("Name", ""),
            new("Status", "Draft")
        };

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Campaigns/Create", formContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        // Debería mostrar error de validación
        Assert.Contains("obligatorio", content, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Fixture con autenticación para pruebas de integración.
/// </summary>
public class AuthenticatedWebApplicationFactoryFixture : IDisposable
{
    public HttpClient Client { get; }
    public string BaseUrl { get; }

    public AuthenticatedWebApplicationFactoryFixture()
    {
        BaseUrl = "http://localhost:56610"; // Puerto según launchSettings.json
        Client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/html"));

        // Hacer login y obtener cookie de autenticación
        LoginAsync().Wait();
    }

    private async Task LoginAsync()
    {
        var loginData = new List<KeyValuePair<string, string>>
        {
            new("Email", "admin@test.com"),
            new("Password", "Admin123!")
        };

        var formContent = new FormUrlEncodedContent(loginData);
        var response = await Client.PostAsync("/Account/Login", formContent);

        // Extraer cookies de autenticación si es necesario
        if (response.Headers.Contains("Set-Cookie"))
        {
            var cookies = response.Headers.GetValues("Set-Cookie");
            // Configurar cookies en el cliente para siguientes requests
        }
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}

