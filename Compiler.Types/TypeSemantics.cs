namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// Whether a type is a value type or a reference type.
/// </summary>
public enum TypeSemantics
{
    Reference = 1,
    // TODO add Hybrid
    Value,
}
