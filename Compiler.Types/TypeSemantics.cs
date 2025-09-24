namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The kind of semantics that a type has. Whether and when it is pass by value or pass by reference.
/// </summary>
public enum TypeSemantics
{
    Reference = 1,
    Hybrid,
    Value,
}
