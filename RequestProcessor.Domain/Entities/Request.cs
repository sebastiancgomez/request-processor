namespace RequestProcessor.Domain.Entities;

using RequestProcessor.Domain.Exceptions;

public enum RequestStatus { Pending, Processed, Failed }

public class ProcessingRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public void ChangeToProcessed()
    {
        if (this.Status != RequestStatus.Pending)
            throw new BusinessException($"Request can not change from {this.Status.ToString()} to {RequestStatus.Processed.ToString()}");
        
        this.Status = RequestStatus.Processed;

    }
    public void ChangeToFailed()
    {
        if (this.Status != RequestStatus.Pending)
            throw new BusinessException($"Request can not change from {this.Status.ToString()} to {RequestStatus.Failed.ToString()}");

        this.Status = RequestStatus.Failed;

    }
}
