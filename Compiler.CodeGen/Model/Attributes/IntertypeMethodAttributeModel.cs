using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public class IntertypeMethodAttributeModel : AspectAttributeModel
{
    public override IntertypeMethodAttributeSyntax? Syntax { get; }
    public override bool IsChild => false;
    public string Parameters { get; }
    public override TypeModel Type { get; }
    public string? DefaultExpression { get; }

    public IntertypeMethodAttributeModel(AspectModel aspect, IntertypeMethodAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, true)
    {
        Syntax = syntax;
        Parameters = syntax.Parameters;
        Type = TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type);
        DefaultExpression = syntax.DefaultExpression;
    }

    public override string ToString()
    {
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"+ {NodeSymbol}.{Name}({Parameters}): {Type}{defaultExpression}";
    }
}
