namespace BuildingBlocks.Exceptions;
/// <summary>
/// Implement common  InternalServer Exception
/// </summary>
public class InternalServerException : Exception
{
    public InternalServerException(string message) : base(message)
    {
    }

    public InternalServerException(string message, string details) : base(message)
    {
        Details = details;
    }
    public string? Details { get; }
}

