using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System;

public class SerilogArgumentNullException : ArgumentNullException
{
    public static void ThrowIfNull(
        [NotNull] object? argument,
        string? errorMessage = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            Log.Error("Argument {paramName} was null!", paramName);
            throw new ArgumentNullException(nameof(argument));
        }
    }
}