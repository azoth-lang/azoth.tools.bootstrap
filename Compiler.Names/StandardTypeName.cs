using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// Name for a type that is not a special name.
/// </summary>
[Closed(
    typeof(IdentifierName),
    typeof(GenericName))]
public abstract class StandardTypeName : TypeName
{
    public static StandardTypeName Create(string text, int genericParameterCount)
        => genericParameterCount == 0 ? new IdentifierName(text) : new GenericName(text, genericParameterCount);

    protected StandardTypeName(string text, int genericParameterCount)
        : base(text, genericParameterCount) { }

    public abstract override StandardTypeName WithAttributeSuffix();

    public override string ToBareString() => QuotedText;

    public static implicit operator StandardTypeName(string text) => new IdentifierName(text);
}
