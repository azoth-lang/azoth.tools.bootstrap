using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class IntertypeMethodEquationModel : SoleEquationModel
{
    public override IntertypeMethodEquationSyntax Syntax { get; }
    public override IntertypeMethodAttributeModel Attribute => attribute.Value;
    private readonly Lazy<IntertypeMethodAttributeModel> attribute;
    public override EvaluationStrategy Strategy => EvaluationStrategy.Computed;
    public override string Parameters { get; }
    public override TypeModel Type => Attribute.Type;
    public override bool RequiresEmitOnNode => Expression is not null;

    public IntertypeMethodEquationModel(AspectModel aspect, IntertypeMethodEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, true,
            syntax.Expression)
    {
        Syntax = syntax;
        Parameters = syntax.Parameters;
        attribute = new(GetAttribute<IntertypeMethodAttributeModel>);
    }

    public override string ToString() => $"= {NodeSymbol}.{Name}({Parameters})";
}
