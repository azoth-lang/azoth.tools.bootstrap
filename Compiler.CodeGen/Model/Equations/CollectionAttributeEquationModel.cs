using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class CollectionAttributeEquationModel : ContributorEquationModel
{
    public override CollectionAttributeEquationSyntax Syntax { get; }
    public CollectionAttributeModel Attribute => attribute.Value;
    private readonly Lazy<CollectionAttributeModel> attribute;
    public Symbol TargetNodeSymbol { get; }
    public TreeNodeModel TargetNode => targetNode.Value;
    private readonly Lazy<TreeNodeModel> targetNode;
    public bool IsForEach => Syntax.IsForEach;
    public string? TargetExpression => Syntax.TargetExpression;
    public override TypeModel Type => Attribute.Type;
    public TypeModel FromType => Attribute.FromType;

    public CollectionAttributeEquationModel(AspectModel aspect, CollectionAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod, syntax.Expression)
    {
        Syntax = syntax;
        TargetNodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.TargetNode);
        targetNode = new(() => Aspect.Tree.NodeFor(NodeSymbol)
                               ?? throw new($"Attribute '{this}' refers to node '{NodeSymbol}' that does not exist."));
        attribute = new(ComputeAttribute);
    }

    private CollectionAttributeModel ComputeAttribute()
        => TargetNode.DeclaredAttributes.OfType<CollectionAttributeModel>().Where(a => a.Name == Name).TrySingle()
           ?? throw new FormatException($"No target attribute for collection attribute equation {this}");

    public override string ToString()
    {
        var each = IsForEach ? " each" : "";
        var @for = TargetExpression is not null ? $" for{each} {TargetExpression}" : "";
        return $"= {NodeSymbol}.→*.{TargetNodeSymbol}.{Name}{@for}";
    }
}
