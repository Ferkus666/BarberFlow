using System.Net.Mime;
using BarberFlow.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Regra de negócio rejeitada. TraceId: {TraceId}", context.TraceIdentifier);
            await ResponderAsync(context, StatusCodes.Status400BadRequest, "Operação não permitida", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado. TraceId: {TraceId}", context.TraceIdentifier);
            await ResponderAsync(context, StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado", "Não foi possível concluir a operação. Tente novamente.");
        }
    }

    private static async Task ResponderAsync(HttpContext context, int statusCode, string title, string detail)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        var aceitaJson = context.Request.Headers.Accept.Any(value =>
            value.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase));

        if (context.Request.Path.StartsWithSegments("/api") || aceitaJson)
        {
            context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = context.TraceIdentifier }
            });
            return;
        }

        context.Response.Redirect($"/Home/Erro?statusCode={statusCode}&traceId={Uri.EscapeDataString(context.TraceIdentifier)}");
    }
}
