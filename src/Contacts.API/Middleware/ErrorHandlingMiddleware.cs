using System.Text.Json;
using Contacts.Domain.Exceptions;

namespace Contacts.API.Middleware;

/// <summary>
/// Centraliza a tradução de exceções conhecidas para respostas HTTP padronizadas.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions _writeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Inicializa o middleware com o próximo delegate e o logger da pipeline.
    /// </summary>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processa a requisição HTTP e converte exceções em respostas JSON.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConflictException ex)
        {
            await WriteJson(context, StatusCodes.Status409Conflict,
                new { type = "conflict_error", errors = new[] { ex.Error } });
        }
        catch (DomainException ex)
        {
            await WriteJson(context, StatusCodes.Status400BadRequest,
                new { type = "validation_error", errors = ex.Errors });
        }
        catch (JsonException)
        {
            // Erro de desserialização JSON, por exemplo enum com string inválida.
            await WriteJson(context, StatusCodes.Status400BadRequest,
                new { type = "validation_error", errors = new[] { "The request body contains invalid or malformed data." } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteJson(context, StatusCodes.Status500InternalServerError,
                new { type = "internal_error", errors = new[] { "An unexpected error occurred." } });
        }
    }

    private static async Task WriteJson(HttpContext context, int statusCode, object body)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, _writeOptions));
    }
}
