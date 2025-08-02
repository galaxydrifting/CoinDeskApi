using System.Text;

namespace CoinDeskApi.Api.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request
            await LogRequest(context);

            // Capture response
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            var startTime = DateTime.UtcNow;

            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                // 如果發生例外，恢復原始 response stream 讓 GlobalExceptionMiddleware 可以處理
                context.Response.Body = originalResponseBodyStream;
                throw; // 重新拋出例外讓 GlobalExceptionMiddleware 處理
            }

            var endTime = DateTime.UtcNow;

            // Log response
            await LogResponse(context, responseBodyStream, endTime - startTime);

            // Copy response back to original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }

        private async Task LogRequest(HttpContext context)
        {
            var request = context.Request;

            var requestLog = new
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = await GetRequestBody(request)
            };

            _logger.LogInformation("HTTP Request: {@RequestLog}", requestLog);
        }

        private async Task LogResponse(HttpContext context, MemoryStream responseBodyStream, TimeSpan duration)
        {
            var response = context.Response;
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            var responseLog = new
            {
                StatusCode = response.StatusCode,
                Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = responseBody,
                Duration = duration.TotalMilliseconds
            };

            _logger.LogInformation("HTTP Response: {@ResponseLog}", responseLog);
        }

        private static async Task<string> GetRequestBody(HttpRequest request)
        {
            if (request.ContentLength == null || request.ContentLength == 0)
                return string.Empty;

            request.EnableBuffering();
            var buffer = new byte[request.ContentLength.Value];
            await request.Body.ReadExactlyAsync(buffer, 0, buffer.Length);
            request.Body.Position = 0;

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
