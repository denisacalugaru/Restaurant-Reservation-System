using System.Net;
using System.Text.Json;

namespace RestaurantReservation.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "A apărut o eroare internă la server."; //500

            if (ex is KeyNotFoundException || ex.Message.Contains("nu a fost găsită") || ex.Message.Contains("not found")) //404
            {
                statusCode = HttpStatusCode.NotFound;
                message = ex.Message;
            }
            else if (ex is ArgumentException || ex is BadHttpRequestException)
            {
                statusCode = HttpStatusCode.BadRequest; //400
                message = ex.Message;
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Forbidden; //403
                message = "Nu aveți permisiunea de a accesa această resursă.";
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}