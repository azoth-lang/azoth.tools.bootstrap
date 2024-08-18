namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class DescendantsSelectorSyntax : SelectorSyntax
{
    #region Singleton
    public static DescendantsSelectorSyntax Instance { get; } = new();

    private DescendantsSelectorSyntax() { }
    #endregion

    public override string ToString() => "**";
}
