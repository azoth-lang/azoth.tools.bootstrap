using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class AspectModel : IHasUsingNamespaces
{
    public TreeModel Tree { get; }
    public AspectSyntax Syntax { get; }
    public string Namespace => Syntax.Namespace;
    public string Name => Syntax.Name;
    public IFixedSet<string> UsingNamespaces => Syntax.UsingNamespaces;
    public IFixedList<AttributeModel> Attributes { get; }
    public IFixedList<EquationModel> Equations { get; }

    public AspectModel(TreeModel tree, AspectSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        Attributes = syntax.Attributes.Select(a => AttributeModel.Create(this, a)).ToFixedList();
        Equations = syntax.Equations.Select(e => EquationModel.Create(this, e)).ToFixedList();
    }
}
