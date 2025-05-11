namespace ECommerce.API.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Swagger UI ve Swagger JSON için bypass
            if (path?.StartsWith("/swagger") == true || path?.Contains("swagger") == true)
            {
                await _next(context);
                return;
            }


            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            var configuredApiKey = _configuration.GetValue<string>("ApiSettings:ApiKey");

            if (!string.Equals(extractedApiKey, configuredApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}
