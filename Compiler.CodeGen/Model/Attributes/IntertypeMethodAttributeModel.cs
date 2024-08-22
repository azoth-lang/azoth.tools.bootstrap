using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public class IntertypeMethodAttributeModel : AspectAttributeModel
{
    public override IntertypeMethodAttributeSyntax? Syntax { get; }
    public string Parameters { get; }
    public string? DefaultExpression { get; }

    public IntertypeMethodAttributeModel(AspectModel aspect, IntertypeMethodAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            true, TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type))
    {
        Syntax = syntax;
        Parameters = syntax.Parameters;
        DefaultExpression = syntax.DefaultExpression;
    }

    public override string ToString()
    {
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"+ {NodeSymbol}.{Name}({Parameters}): {Type}{defaultExpression}";
    }
}
