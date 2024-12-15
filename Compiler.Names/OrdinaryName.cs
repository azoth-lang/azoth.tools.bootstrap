using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// Name that is not a built-in name. (Could be generic or non-generic.)
/// </summary>
[Closed(
    typeof(IdentifierName),
    typeof(GenericName))]
public abstract class OrdinaryName : TypeName
{
    public static OrdinaryName Create(string text, int genericParameterCount)
        => genericParameterCount == 0 ? new IdentifierName(text) : new GenericName(text, genericParameterCount);

    private protected OrdinaryName(string text, int genericParameterCount)
        : base(text, genericParameterCount) { }

    public abstract override OrdinaryName WithAttributeSuffix();

    public override string ToBareString() => QuotedText;

    public static implicit operator OrdinaryName(string text) => new IdentifierName(text);
}
