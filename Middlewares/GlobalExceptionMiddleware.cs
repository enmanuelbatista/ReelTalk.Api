using System.Net;
using System.Text.Json;

namespace ReelTalk.Api.Middlewares;

public class GlobalExceptionMiddleware
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
            // Continuar con la siguiente petición en el pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Si ocurre cualquier error no controlado, se captura aquí
            _logger.LogError(ex, "Ocurrió un error no controlado en el servidor: {Message}", ex.Message);
            await ManejarExcepcionAsync(context, ex);
        }
    }

    private static Task ManejarExcepcionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Estructura estandarizada de respuesta de error
        var respuestaError = new
        {
            statusCode = context.Response.StatusCode,
            mensaje = "Ocurrió un error interno en el servidor. Por favor, intente más tarde.",
            detalle = exception.Message // Puedes remover el detalle en producción si deseas
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonRespuesta = JsonSerializer.Serialize(respuestaError, jsonOptions);

        return context.Response.WriteAsync(jsonRespuesta);
    }
}
