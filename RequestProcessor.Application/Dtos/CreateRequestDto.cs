namespace RequestProcessor.Application.Dtos;

public class CreateRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}
