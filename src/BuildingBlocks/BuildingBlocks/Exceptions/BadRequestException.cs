namespace BuildingBlocks.Exceptions;
/// <summary>
/// Implement common bad request exception
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, string details) : base(message)
    {
        Details = details;
    }
    public string? Details { get; }
}

