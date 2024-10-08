using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class CollectionAttributeModel : AspectAttributeModel
{
    public override CollectionAttributeSyntax Syntax { get; }
    public CollectionAttributeFamilyModel Family => family.Value;
    private readonly Lazy<CollectionAttributeFamilyModel> family;
    public override TypeModel Type { get; }
    public Symbol? RootSymbol { get; }
    public TypeModel FromType { get; }
    public string ConstructExpression { get; }
    public string DoneMethod => Syntax.DoneMethod;

    public CollectionAttributeModel(AspectModel aspect, CollectionAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod)
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type);
        RootSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Root);
        FromType = TypeModel.CreateFromSyntax(aspect.Tree, syntax.FromType);
        ConstructExpression = Syntax.ConstructExpression ?? $"new {Emit.Type(FromType)}()";
        family = new(ComputeAttributeFamily);
    }

    private CollectionAttributeFamilyModel ComputeAttributeFamily()
        => Aspect.Tree.AllAttributeFamilies
                 .OfType<CollectionAttributeFamilyModel>()
                 .Single(f => f.Instances.Contains(this));

    public override string ToString()
    {
        var root = RootSymbol is not null ? $" root {RootSymbol}" : "";
        return $"→*← {NodeSymbol}.{Name}: {Type}{root} from {FromType} => {ConstructExpression} done {DoneMethod}";
    }
}
