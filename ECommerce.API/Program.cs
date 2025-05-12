using ECommerce.API.Dtos;
using ECommerce.API.Middleware;
using ECommerce.API.Settings;
using ECommerce.API.Validators;
using ECommerce.Application.Interfaces.Concurrency;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Persistence;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

var appSettings = builder.Configuration
    .Get<AppSettings>();


builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce API",
        Version = "v1",
        Description = "Backend system for processing orders with Balance Management integration."
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = $"API Key needed to access the endpoints. its value for development env: {builder.Configuration.GetValue<string>("ApiSettings:ApiKey")}",
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
                { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } },
            new string[] { }
        }
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
            MaxRetryAttempts = 5,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            OnRetry = args =>
            {
                Log.Information($"[Retry Attempt {args.AttemptNumber}] at {DateTime.UtcNow}. Reason: {args.Outcome.Exception?.Message}");
                return default;
            },
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
                Log.Information($"[Circuit OPENED] at {DateTime.UtcNow} due to: {args.Outcome.Exception?.Message}");
                return default;
            },
            OnClosed = args =>
            {
                Log.Information($"[Circuit CLOSED] at {DateTime.UtcNow}");
                return default;
            },
            OnHalfOpened = args =>
            {
                Log.Information($"[Circuit HALF-OPEN] at {DateTime.UtcNow}");
                return default;
            }
        });
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<ILockProvider, LockProvider>();
//builder.Services.AddSingleton<IOrderRepository, InMemoryOrderDictionaryRepository>();


var connection = new SqliteConnection("datasource=:memory:");
connection.Open();

builder.Services.AddDbContext<ECommerceDbContext>(options =>
{
    options.UseSqlite(connection);
});

builder.Services.AddScoped<IOrderRepository, InMemorySqliteRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IOrderManager,  OrderManager>();


var app = builder.Build();

// Ensure the in-memory SQLite database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    dbContext.Database.EnsureCreated();  // Veritabaný ve tablolarý oluþturuyor


}

// Middleware
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();
app.Run();

