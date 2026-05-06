using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using RequestProcessor.Application.Interfaces;
using System.Text.Json;

namespace RequestProcessor.Infrastructure.Messaging;

public class QueueStoragePublisher : IMessagePublisher
{
    private readonly QueueClient _queueClient;

    public QueueStoragePublisher(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");
        var queueName = configuration["Messaging:QueueName"] ?? "request-created-queue";

        var options = new QueueClientOptions(QueueClientOptions.ServiceVersion.V2024_05_04);
        _queueClient = new QueueClient(connectionString, queueName, options);
    }

    public async Task PublishRequestCreatedEventAsync(Guid requestId, DateTime createdAt)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync();

            var eventPayload = new
            {
                Id = requestId,
                CreatedAt = createdAt,
                EventType = "RequestCreated"
            };

            string messageBody = JsonSerializer.Serialize(eventPayload);

            string base64Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageBody));

        
            await _queueClient.SendMessageAsync(base64Message);            
        }
        catch (Azure.RequestFailedException ex)
        {
            throw;
        }
    }
}