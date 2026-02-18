using System.Reflection;

namespace TOTDistribution.NadeoRefinery.Common.Extensions;
public static class TypeReflection
{
    public static ServiceDescriptor[] GetServiceSlicesAsArray(
        this IEnumerable<TypeInfo> types, Type typeToFind)
    {
        var matchingTypes = types
            .Where(type => type.IsAssignableTo(typeToFind));

        var descriptors = matchingTypes
            .Select(type =>
                ServiceDescriptor.Transient(typeToFind, type));

        return [.. descriptors];
    }
}