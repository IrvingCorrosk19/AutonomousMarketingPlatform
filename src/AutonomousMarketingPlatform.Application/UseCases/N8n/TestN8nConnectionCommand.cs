using AutonomousMarketingPlatform.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;

namespace AutonomousMarketingPlatform.Application.UseCases.N8n;

/// <summary>
/// Command para probar la conexión con n8n.
/// </summary>
public class TestN8nConnectionCommand : IRequest<TestN8nConnectionResponse>
{
    public TestN8nConnectionDto Request { get; set; } = null!;
}

/// <summary>
/// Handler para probar la conexión con n8n.
/// </summary>
public class TestN8nConnectionCommandHandler : IRequestHandler<TestN8nConnectionCommand, TestN8nConnectionResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TestN8nConnectionCommandHandler> _logger;

    public TestN8nConnectionCommandHandler(
        IHttpClientFactory httpClientFactory,
        ILogger<TestN8nConnectionCommandHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<TestN8nConnectionResponse> Handle(TestN8nConnectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            // Intentar conectar a la URL base de n8n
            var baseUrl = request.Request.BaseUrl.TrimEnd('/');
            var testUrl = $"{baseUrl}/healthz"; // n8n tiene un endpoint de health

            if (string.IsNullOrWhiteSpace(request.Request.BaseUrl))
            {
                return new TestN8nConnectionResponse
                {
                    Success = false,
                    Message = "URL base no puede estar vacía",
                    Error = "BaseUrl is required"
                };
            }

            _logger.LogInformation("Probando conexión con n8n en: {Url}", testUrl);

            HttpResponseMessage? response = null;
            try
            {
                response = await httpClient.GetAsync(testUrl, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Error al conectar con n8n: {Url}", testUrl);
                return new TestN8nConnectionResponse
                {
                    Success = false,
                    Message = $"No se pudo conectar con n8n: {ex.Message}",
                    Error = ex.Message,
                    StatusCode = null
                };
            }
            catch (TaskCanceledException)
            {
                return new TestN8nConnectionResponse
                {
                    Success = false,
                    Message = "Timeout al conectar con n8n. Verifica que n8n esté corriendo.",
                    Error = "Connection timeout"
                };
            }

            if (response != null)
            {
                var statusCode = (int)response.StatusCode;
                var isSuccess = response.IsSuccessStatusCode;

                if (isSuccess)
                {
                    _logger.LogInformation("Conexión exitosa con n8n: StatusCode={StatusCode}", statusCode);
                    return new TestN8nConnectionResponse
                    {
                        Success = true,
                        Message = $"Conexión exitosa con n8n (Status: {statusCode})",
                        StatusCode = statusCode
                    };
                }
                else
                {
                    _logger.LogWarning("n8n respondió con error: StatusCode={StatusCode}", statusCode);
                    return new TestN8nConnectionResponse
                    {
                        Success = false,
                        Message = $"n8n respondió con error (Status: {statusCode})",
                        StatusCode = statusCode
                    };
                }
            }

            return new TestN8nConnectionResponse
            {
                Success = false,
                Message = "No se recibió respuesta de n8n",
                Error = "No response"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al probar conexión con n8n");
            return new TestN8nConnectionResponse
            {
                Success = false,
                Message = $"Error inesperado: {ex.Message}",
                Error = ex.Message
            };
        }
    }
}

