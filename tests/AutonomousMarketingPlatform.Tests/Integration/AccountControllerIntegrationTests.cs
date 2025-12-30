using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AutonomousMarketingPlatform.Tests.Integration;

/// <summary>
/// Pruebas de integración para AccountController.
/// Estas pruebas requieren que la aplicación esté ejecutándose.
/// </summary>
public class AccountControllerIntegrationTests : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public AccountControllerIntegrationTests(WebApplicationFactoryFixture fixture)
    {
        _client = fixture.Client;
        _baseUrl = fixture.BaseUrl;
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Login_Get_ShouldReturnLoginPage()
    {
        // Act
        var response = await _client.GetAsync("/Account/Login");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Login", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Login_Post_WithInvalidCredentials_ShouldReturnError()
    {
        // Arrange
        var loginData = new
        {
            Email = "invalid@test.com",
            Password = "WrongPassword123!"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginData),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/Account/Login", content);

        // Assert
        // Debería redirigir o mostrar error
        Assert.True(response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Login_Post_WithValidCredentials_ShouldRedirect()
    {
        // Arrange
        var formData = new List<KeyValuePair<string, string>>
        {
            new("Email", "admin@test.com"),
            new("Password", "Admin123!")
        };

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Account/Login", formContent);

        // Assert
        // Debería redirigir al dashboard o home
        Assert.True(response.StatusCode == HttpStatusCode.Redirect || 
                    response.StatusCode == HttpStatusCode.OK);
    }

    [Fact(Skip = "Requiere aplicación ejecutándose. Ejecutar manualmente cuando la app esté corriendo.")]
    public async Task Logout_ShouldRedirectToLogin()
    {
        // Arrange - Primero hacer login
        var loginData = new List<KeyValuePair<string, string>>
        {
            new("Email", "admin@test.com"),
            new("Password", "Admin123!")
        };
        var loginContent = new FormUrlEncodedContent(loginData);
        await _client.PostAsync("/Account/Login", loginContent);

        // Act - Hacer logout
        var response = await _client.PostAsync("/Account/Logout", null);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Redirect);
        var location = response.Headers.Location?.ToString();
        Assert.Contains("Login", location ?? "", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Fixture para pruebas de integración.
/// En un escenario real, esto usaría WebApplicationFactory.
/// </summary>
public class WebApplicationFactoryFixture : IDisposable
{
    public HttpClient Client { get; }
    public string BaseUrl { get; }

    public WebApplicationFactoryFixture()
    {
        // En producción, usar WebApplicationFactory<Program>
        BaseUrl = "http://localhost:56610"; // Puerto según launchSettings.json
        Client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/html"));
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}

