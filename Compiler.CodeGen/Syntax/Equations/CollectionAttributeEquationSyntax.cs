namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class CollectionAttributeEquationSyntax : EquationSyntax
{
    public SymbolSyntax TargetNode { get; }
    public bool IsForEach { get; }
    public string? TargetExpression { get; }

    public CollectionAttributeEquationSyntax(
            SymbolSyntax node,
            SymbolSyntax targetNode,
            string name,
            bool isForEach,
            string? targetExpression)
        // The expression of the base class is for the body of the equation, not the target.
        : base(node, name, false, null)
    {
        TargetNode = targetNode;
        IsForEach = isForEach;
        TargetExpression = targetExpression;
    }

    public override string ToString()
    {
        var each = IsForEach ? " each" : "";
        var @for = TargetExpression is not null ? $" for{each} {TargetExpression}" : "";
        return $"= {Node}.→*.{TargetNode}.{Name}{@for}";
    }
}
