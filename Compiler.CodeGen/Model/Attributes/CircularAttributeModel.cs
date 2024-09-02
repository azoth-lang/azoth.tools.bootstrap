using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class CircularAttributeModel : LocalAttributeModel
{
    public override CircularAttributeSyntax Syntax { get; }
    public override EvaluationStrategy Strategy => EvaluationStrategy.Lazy;
    public override TypeModel Type { get; }
    public string? InitialExpression { get; }

    public CircularAttributeModel(AspectModel aspect, CircularAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, syntax.DefaultExpression)
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type);
        InitialExpression = syntax.InitialExpression;
    }

    public override string ToString()
    {
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"⟳ {NodeSymbol}.{Name}: {Type}{defaultExpression};";
    }
}
