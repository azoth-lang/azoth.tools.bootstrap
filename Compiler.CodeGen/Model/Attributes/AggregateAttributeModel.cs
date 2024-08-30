using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class AggregateAttributeModel : AspectAttributeModel
{
    public override AggregateAttributeSyntax? Syntax { get; }
    public override TypeModel Type => AttributeFamily.Type;
    public TypeModel FromType => AttributeFamily.FromType;
    public AggregateAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<AggregateAttributeFamilyModel> attributeFamily;
    public string AggregateMethod => AttributeFamily.AggregateMethod;

    public AggregateAttributeModel(AspectModel aspect, AggregateAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, false)
    {
        Syntax = syntax;
        attributeFamily = new(ComputeAttributeFamily<AggregateAttributeFamilyModel>);
    }

    public override string ToString() => $"↗↖ {Node}.{Name}";
}
