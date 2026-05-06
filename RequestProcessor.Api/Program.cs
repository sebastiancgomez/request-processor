using Azure.Messaging.ServiceBus;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RequestProcessor.Application.Interfaces;
using RequestProcessor.Application.Services;
using RequestProcessor.Application.Validators;
using RequestProcessor.Domain.Repositories;
using RequestProcessor.Infrastructure.Context;
using RequestProcessor.Infrastructure.Messaging;
using RequestProcessor.Infrastructure.Repositories;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
try
{
    Log.Information("Starting Web Host...");
    builder.Services.AddControllers(); 
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
    var messagingProvider = builder.Configuration["Messaging:Provider"] ?? "ServiceBus";
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateRequestValidator>();

    builder.Services.AddScoped<IRequestRepository, PostgresRequestRepository>();
    builder.Services.AddScoped<IRequestService, RequestService>();


    if (messagingProvider == "QueueStorage")
    {
        builder.Services.AddScoped<IMessagePublisher, QueueStoragePublisher>();
    }
    else
    {
        builder.Services.AddScoped<IMessagePublisher, AzureServiceBusPublisher>();
    }
    var app = builder.Build();
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
                Console.WriteLine("Database migrations applied successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex.Message}");
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Request Processor API V1");
        });
    }



    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host ended unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}