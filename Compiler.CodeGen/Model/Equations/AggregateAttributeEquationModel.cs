using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class AggregateAttributeEquationModel : EquationModel
{
    public override AggregateAttributeEquationSyntax Syntax { get; }
    public override AggregateAttributeModel Attribute => attribute.Value;
    private readonly Lazy<AggregateAttributeModel> attribute;
    public AggregateAttributeFamilyModel AttributeFamily => Attribute.AttributeFamily;
    public override TypeModel Type => Attribute.Type;

    public AggregateAttributeEquationModel(AspectModel aspect, AggregateAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, false, null)
    {
        Syntax = syntax;
        attribute = new(GetAttribute<AggregateAttributeModel>);
    }

    public override string ToString() => $"= {NodeSymbol}.↑.{Name}";
}
