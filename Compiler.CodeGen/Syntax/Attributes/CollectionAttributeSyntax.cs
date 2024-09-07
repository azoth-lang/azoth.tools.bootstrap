using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class CollectionAttributeSyntax : AspectAttributeSyntax
{
    public override TypeSyntax Type { get; }
    public SymbolSyntax? Root { get; }
    public TypeSyntax FromType { get; }
    public string? ConstructExpression { get; }
    public string DoneMethod { get; }

    public CollectionAttributeSyntax(
        SymbolSyntax node,
        string name,
        TypeSyntax type,
        SymbolSyntax? root,
        TypeSyntax fromType,
        string? constructExpression,
        string doneMethod)
        : base(false, EvaluationStrategy.Lazy, node, name, false)
    {
        Type = type;
        Root = root;
        FromType = fromType;
        ConstructExpression = constructExpression;
        DoneMethod = doneMethod;
    }

    public override string ToString()
    {
        var root = Root is not null ? $" root {Root}" : "";
        var construct = ConstructExpression is not null ? $" => {ConstructExpression}" : "";
        return $"→*← {Node}.{Name}: {Type}{root} from {FromType}{construct} done {DoneMethod}";
    }
}
