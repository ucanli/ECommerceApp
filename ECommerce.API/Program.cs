using ECommerce.API.Settings;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Infrastructure.Services;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
using Polly;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration
    .Get<AppSettings>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce API",
        Version = "v1",
        Description = "Backend system for processing orders with Balance Management integration."
    });
});


builder.Services.AddHttpClient("balance-management-api", (serviceProvider, client) =>
    {
        client.BaseAddress = new Uri(appSettings.BalanceManagementApiBaseUrl);
    }).AddResilienceHandler("my-pipeline", builder =>
    {
        //Retry
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(200),
            BackoffType = DelayBackoffType.Exponential,
            OnRetry = args =>
            {
                Console.WriteLine($"[Retry Attempt {args.AttemptNumber}] at {DateTime.UtcNow}. Reason: {args.Outcome.Exception?.Message}");
                return default;
            }
        });

        //Timeout
        builder.AddTimeout(TimeSpan.FromSeconds(5));

        //Circuit Breaker: 30 saniyede 10 çaðrýdan %50'si hatalýysa, 15 saniye boyunca trafiði kes
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(10),
            FailureRatio = 0.5,
            MinimumThroughput = 10,
            BreakDuration = TimeSpan.FromSeconds(15),
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                 .Handle<HttpRequestException>() // Includes any HttpRequestException
                 .HandleResult(response => !response.IsSuccessStatusCode), // Includes non-successful responses
            OnOpened = args =>
            {
                Console.WriteLine($"[Circuit OPENED] at {DateTime.UtcNow} due to: {args.Outcome.Exception?.Message}");
                return default;
            },
            OnClosed = args =>
            {
                Console.WriteLine($"[Circuit CLOSED] at {DateTime.UtcNow}");
                return default;
            },
            OnHalfOpened = args =>
            {
                Console.WriteLine($"[Circuit HALF-OPEN] at {DateTime.UtcNow}");
                return default;
            }
        });
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IProductManager, ProductManager>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();
app.Run();

