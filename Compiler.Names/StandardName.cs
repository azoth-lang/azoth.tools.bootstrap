using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// Name that is not a special name. (Could be generic or non-generic.)
/// </summary>
[Closed(
    typeof(IdentifierName),
    typeof(GenericName))]
public abstract class StandardName : TypeName
{
    public static StandardName Create(string text, int genericParameterCount)
        => genericParameterCount == 0 ? new IdentifierName(text) : new GenericName(text, genericParameterCount);

    protected StandardName(string text, int genericParameterCount)
        : base(text, genericParameterCount) { }

    public abstract override StandardName WithAttributeSuffix();

    public override string ToBareString() => QuotedText;

    public static implicit operator StandardName(string text) => new IdentifierName(text);
}
