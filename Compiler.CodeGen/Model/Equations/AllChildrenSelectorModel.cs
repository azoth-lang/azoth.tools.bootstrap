using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class AllChildrenSelectorModel : SelectorModel
{
    #region Singleton
    public static AllChildrenSelectorModel Instance { get; } = new();

    private AllChildrenSelectorModel() { }
    #endregion

    public override AllChildrenSelectorSyntax Syntax => AllChildrenSelectorSyntax.Instance;
}
