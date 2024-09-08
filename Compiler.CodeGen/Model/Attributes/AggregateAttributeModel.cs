using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class AggregateAttributeModel : AspectAttributeModel
{
    public override AggregateAttributeSyntax? Syntax { get; }
    public override TypeModel Type => Family.Type;
    public TypeModel FromType => Family.FromType;
    public AggregateAttributeFamilyModel Family => family.Value;
    private readonly Lazy<AggregateAttributeFamilyModel> family;
    public string AggregateMethod => Family.AggregateMethod;

    public AggregateAttributeModel(AspectModel aspect, AggregateAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, false)
    {
        Syntax = syntax;
        family = new(ComputeAttributeFamily<AggregateAttributeFamilyModel>);
    }

    public override string ToString() => $"↗↖ {Node}.{Name}";
}
