using System.Reflection;

namespace TOTDBackend.NadeoRefinery.Common.Extensions;

public static class TypeReflection
{
    public static ServiceDescriptor[] GetServiceSlicesAsArray(
        this IEnumerable<TypeInfo> types, Type typeToFind)
    {
        var descriptors = types
            .Where(typeToFind.IsAssignableFrom)
            .Select(type => ServiceDescriptor.Transient(typeToFind, type));

        return [.. descriptors];
    }
}