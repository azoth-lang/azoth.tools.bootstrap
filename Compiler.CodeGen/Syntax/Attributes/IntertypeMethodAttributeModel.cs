using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

internal class IntertypeMethodAttributeModel : AspectAttributeModel
{
    public override IntertypeMethodAttributeSyntax? Syntax { get; }
    public string Parameters { get; }
    public string Expression { get; }

    public IntertypeMethodAttributeModel(AspectModel aspect, IntertypeMethodAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            true, TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type))
    {
        Syntax = syntax;
        Parameters = syntax.Parameters;
        Expression = syntax.Expression;
    }

    public override string ToString() => $"+ {NodeSymbol}.{Name}({Parameters}): {Type} => {Expression}";
}
