using ECommerce.API.Settings;
using ECommerce.Application.Interfaces.External;
using ECommerce.Infrastructure.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration
    .Get<AppSettings>();

Console.WriteLine("appSettings?.BalanceManagementApiBaseUrl");
Console.WriteLine(appSettings?.BalanceManagementApiBaseUrl);

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
});

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();
app.Run();
