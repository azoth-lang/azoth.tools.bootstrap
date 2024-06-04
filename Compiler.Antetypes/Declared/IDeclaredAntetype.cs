namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

/// <summary>
/// An antetype as it is declared.
/// </summary>
/// <remarks>For generic types, a declared type is not a type, but rather a template or kind for
/// creating types.</remarks>
public interface IDeclaredAntetype
{
    IAntetype With(IEnumerable<IAntetype> typeArguments);
}