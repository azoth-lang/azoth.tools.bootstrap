using System;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// Indicates that the operation has failed because the struct it was called on was not initialized.
/// </summary>
public class StructNotInitializedException : Exception
{
    public StructNotInitializedException(Type structType)
        : base(FormatMessage(structType))
    {
    }

    private static string FormatMessage(Type structType)
        => $"The struct {structType.GetFriendlyName()} was not initialized.";
}
