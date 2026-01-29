namespace TOTDistribution.NadeoRefinery.Features;

/// <summary>
/// Converts types from Nadeo API responses to internal entity models.
/// </summary>
/// <typeparam name="TNadeo">A response type defined in the ManiaAPI.NadeoAPI package</typeparam>
/// <typeparam name="TModel">An entity type </typeparam>
public interface IFormatter<TNadeo, TModel>
{
    TModel Format(TNadeo input);
}