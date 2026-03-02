namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <summary>
/// Interface for Nadeo vertical slices that can use a database as storage in their components.
/// </summary>
/// <typeparam name="TData">Type to store into the database</typeparam>
internal interface ISliceStorable<TData>
    where TData : notnull
{
    Task HandleStorageAsync(TData data, TimeSpan? expiry = null);
    TData HandleRetrieval(string key);
}