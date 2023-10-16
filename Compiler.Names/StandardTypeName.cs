using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// Name for a type that is not a special name.
/// </summary>
[Closed(
    typeof(SimpleName),
    typeof(GenericTypeName))]
public abstract class StandardTypeName : Name
{
    public static StandardTypeName Create(string text, int genericParameterCount)
        => genericParameterCount == 0 ? new SimpleName(text) : new GenericTypeName(text, genericParameterCount);

    protected StandardTypeName(string text, int genericParameterCount)
        : base(text, genericParameterCount) { }

    public override string ToBareString() => QuotedText;

    public static implicit operator StandardTypeName(string text) => new SimpleName(text);
}
