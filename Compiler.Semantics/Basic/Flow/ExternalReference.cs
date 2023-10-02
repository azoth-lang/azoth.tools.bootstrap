using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Represents the possibility that there are external references to parameters of a method.
/// </summary>
public readonly struct ExternalReference
{
    public static readonly ExternalReference NonParameters = new(-1);

    public static ExternalReference CreateLentParameter(long number)
    {
        if (number <= 0)
            throw new ArgumentOutOfRangeException(nameof(number), "Lent group numbers must be > 0.");
        return new ExternalReference(-number - 1);
    }

    public long Number => -Id - 1;

    public long Id { get; }

    private ExternalReference(long id)
    {
        Id = id;
    }

    public override string ToString() => ToString(Id);

    public static string ToString(long number)
    {
        if (number >= 0)
            throw new ArgumentOutOfRangeException(nameof(number), "External reference numbers must be negative");
        return number == -1 ? "⧼params⧽" : $"⧼lent-param{-number - 1}⧽";
    }
}
