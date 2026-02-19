using System.Reflection;

namespace TOTDBackend.NadeoRefinery.Common.Extensions;
public static class TypeReflection
{
    public static ServiceDescriptor[] GetServiceSlicesAsArray(
        this IEnumerable<TypeInfo> types, Type typeToFind)
    {
        var matchingTypes = types
            .Where(typeToFind.IsAssignableFrom);

        var descriptors = matchingTypes
            .Select(type => ServiceDescriptor.Transient(typeToFind, type));

        return [.. descriptors];
    }
}