using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Represents the possibility that there are external references to parameters of a method.
/// </summary>
public readonly struct ExternalReference
{
    public static readonly ExternalReference NonLentGroup = new ExternalReference(-1);

    public static ExternalReference CreateLentGroup(long number)
    {
        if (number <= 0)
            throw new ArgumentOutOfRangeException(nameof(number), "Lent group numbers must be > 0.");
        return new ExternalReference(-number - 1);
    }

    public long Number { get; }

    private ExternalReference(long number)
    {
        Number = number;
    }

    public override string ToString() => ToString(Number);

    public static string ToString(long number)
    {
        if (number >= 0)
            throw new ArgumentOutOfRangeException(nameof(number), "External reference numbers must be negative");
        return number == -1 ? "⧼parameters⧽" : $"⧼lent{-number - 1}⧽";
    }
}
