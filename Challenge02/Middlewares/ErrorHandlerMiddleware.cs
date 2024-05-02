using Amazon.Runtime.Internal;
using Newtonsoft.Json;
using System.Net;
using ILogger = Serilog.ILogger;

namespace Challenge02.Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(
            RequestDelegate next,
            ILogger log)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            //catch (NotFoundException notFoundEx)
            //{
            //    await HandleErrorAsync(httpContext, notFoundEx, HttpStatusCode.NotFound);
            //}
            catch (AggregateException aggEx)
            {
                foreach (var ex in aggEx.InnerExceptions)
                {
                    _logger.Error($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
                await HandleErrorAsync(httpContext, aggEx, HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(httpContext, ex, HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleErrorAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            var errorResponse = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = exception.Message,
                // Adicione mais propriedades conforme necessário
            };

            _logger.Error($"Error: {exception.Message}, StackTrace: {exception.StackTrace}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
