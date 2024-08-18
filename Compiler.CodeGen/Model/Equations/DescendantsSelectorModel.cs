using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class DescendantsSelectorModel : SelectorModel
{
    #region Singleton
    public static DescendantsSelectorModel Instance { get; } = new();

    private DescendantsSelectorModel() { }
    #endregion

    public override DescendantsSelectorSyntax Syntax => DescendantsSelectorSyntax.Instance;
    public override bool IsAllDescendants => true;
}
