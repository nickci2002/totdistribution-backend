namespace TOTDBackend.Shared.Json;

/// <summary>
/// Interface for ALL primitives.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPrimitiveType<T>
{
    public T Value { get; init; }
}
