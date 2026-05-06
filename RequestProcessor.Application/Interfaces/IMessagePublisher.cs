namespace RequestProcessor.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishRequestCreatedEventAsync(Guid id, DateTime createdAt);
}