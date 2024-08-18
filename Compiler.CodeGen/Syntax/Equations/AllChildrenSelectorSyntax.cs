namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class AllChildrenSelectorSyntax : SelectorSyntax
{
    #region Singleton
    public static AllChildrenSelectorSyntax Instance { get; } = new();

    private AllChildrenSelectorSyntax() { }
    #endregion

    public override string ToString() => "*";
}
