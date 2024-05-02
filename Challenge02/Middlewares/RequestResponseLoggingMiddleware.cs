using System.Text;
using ILogger = Serilog.ILogger;

namespace Challenge02.Api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger log)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);

            var originalBodyStream = context.Response.Body;

            try
            {
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    responseBody.Seek(0, SeekOrigin.Begin);

                    await LogResponseAsync(context, responseBody);

                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            var request = context.Request;

            request.EnableBuffering();

            var requestLog = new StringBuilder();
            requestLog.AppendLine("Incoming Request:");
            requestLog.AppendLine($"HTTP {request.Method} {request.Path}");
            requestLog.AppendLine($"Host: {request.Host}");
            requestLog.AppendLine($"Content-Type: {request.ContentType}");
            requestLog.AppendLine($"Content-Length: {request.ContentLength}");

            if (request.ContentLength != null && request.ContentLength > 0)
            {
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                var requestBody = await reader.ReadToEndAsync();
                requestLog.AppendLine($"Request Body: {requestBody}");
                request.Body.Position = 0;
            }

            _logger.Information(requestLog.ToString());
        }

        private async Task LogResponseAsync(HttpContext context, MemoryStream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();

            var response = context.Response;

            var responseLog = new StringBuilder();
            responseLog.AppendLine("Outgoing Response:");
            responseLog.AppendLine($"HTTP {response.StatusCode}");
            responseLog.AppendLine($"Content-Type: {response.ContentType}");
            responseLog.AppendLine($"Content-Length: {responseBody.Length}");
            responseLog.AppendLine($"Response Body: {responseText}");

            _logger.Information(responseLog.ToString());

            responseBody.Seek(0, SeekOrigin.Begin);
        }
    }
}
