namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
public readonly record struct ParameterType(bool IsLentBinding, DataType Type)
{
    public static readonly ParameterType Int = new(false, DataType.Int);

    public string ToILString() => IsLentBinding ? $"lent {Type.ToILString()}" : Type.ToILString();
}
