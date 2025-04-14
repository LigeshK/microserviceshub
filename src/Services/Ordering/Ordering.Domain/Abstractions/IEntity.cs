namespace Ordering.Domain.Abstractions;

/// <summary>
/// Generic interface for all entities with unique identifier of type T to implement entity classes
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}
/// <summary>
/// IEntity interface with common properties
/// </summary>
public interface IEntity
{
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
