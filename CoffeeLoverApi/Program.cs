using CoffeeLoverApi.Application.Abstractions;
using CoffeeLoverApi.Application.Services;
using CoffeeLoverApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger services (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<ICoffeeMachineRepository, InMemoryCoffeeMachineRepository>();
builder.Services.AddScoped<ICoffeeService, CoffeeService>();

var app = builder.Build();

// Swagger middleware (only in Development is a common pattern)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

// Required for WebApplicationFactory discovery with top-level Program.cs
public partial class Program { }