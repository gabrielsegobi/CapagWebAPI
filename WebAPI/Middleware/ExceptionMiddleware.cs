using Application.Exceptions;
using Application.Exceptions.Base;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware
{
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
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Erro tratado: {Message}", ex.Message);

                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorObj = new
                {
                    status_code = ex.StatusCode,
                    code = ex.ErrorCode,
                    message = ex.Message,
                    errors = ex.Errors?.Any() == true ? ex.Errors : null
                };

                var response = new { error = errorObj };
                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };


                var json = JsonSerializer.Serialize(response,options);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno do servidor");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = JsonSerializer.Serialize(new
                {
                    Message = "Ocorreu um erro interno no servidor." 
                });
                await context.Response.WriteAsync(response);
            }
        }
    }
}
