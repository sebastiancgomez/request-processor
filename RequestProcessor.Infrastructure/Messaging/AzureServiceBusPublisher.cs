using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using RequestProcessor.Application.Interfaces;
using System.Text.Json;

namespace RequestProcessor.Infrastructure.Messaging;

public class AzureServiceBusPublisher : IMessagePublisher
{
    private readonly ServiceBusClient _client;
    private readonly string _queueName;

    public AzureServiceBusPublisher(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureServiceBus");
        _queueName = configuration["Messaging:QueueName"] ?? "request-created-queue";
        _client = new ServiceBusClient(connectionString);
    }

    public async Task PublishRequestCreatedEventAsync(Guid requestId, DateTime createdAt)
    {
        var sender = _client.CreateSender(_queueName);

        var eventPayload = new
        {
            Id = requestId,
            CreatedAt = createdAt,
            EventType = "RequestCreated",
        };

        string messageBody = JsonSerializer.Serialize(eventPayload);
        var message = new ServiceBusMessage(messageBody);

        await sender.SendMessageAsync(message);
    }
}