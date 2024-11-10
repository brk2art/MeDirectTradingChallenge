using MeDirectTradingChallenge.Domain.Repositories;
using MeDirectTradingChallenge.Domain.Services;
using MeDirectTradingChallenge.Infrastructure;
using MeDirectTradingChallenge.Infrastructure.Repositories;
using MeDirectTradingChallenge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Configure RabbitMQ ConnectionFactory and register RabbitMQService
builder.Services.AddSingleton<ConnectionFactory>(provider =>
{
    return new ConnectionFactory
    {
        HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
        UserName = configuration["RabbitMQ:UserName"] ?? "guest",
        Password = configuration["RabbitMQ:Password"] ?? "guest"
    };
});

builder.Services.AddScoped<IMessageQueueService, RabbitMQService>();

// Configure Redis Cache Service
builder.Services.AddSingleton<ICacheService>(provider =>
{
    var redisConnectionString = configuration["Redis:ConnectionString"];
    return new RedisCacheService(redisConnectionString);
});

// Register other application services
builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddScoped<ITradeService, TradeService>();

// Add MVC and Swagger services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
