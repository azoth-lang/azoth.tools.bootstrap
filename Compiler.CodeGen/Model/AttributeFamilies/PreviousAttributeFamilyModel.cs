using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

/// <summary>
/// Provides the for all instances of a previous attribute.
/// </summary>
public sealed class PreviousAttributeFamilyModel : ContextAttributeFamilyModel
{
    public PreviousAttributeFamilySyntax Syntax { get; }
    public override string Name => Syntax.Name;
    public override TypeModel Type { get; }

    public PreviousAttributeFamilyModel(TreeModel tree, PreviousAttributeFamilySyntax syntax)
        : base(tree)
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(tree, syntax.Type);
    }

    public override string ToString() => $"⮡ *.{Name}: {Type}";
}
