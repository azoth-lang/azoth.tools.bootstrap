using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NewObjectExpressionSyntax : DataTypedExpressionSyntax, INewObjectExpressionSyntax
{
    /// <summary>
    /// Note that this could represent a named or unnamed constructor. So
    /// for an unnamed constructor, it is really the type name. Conceptually
    /// though, the type name is the name of the unnamed constructor. Thus,
    /// this expression's type could be either an object type, or member type.
    /// </summary>
    public ITypeNameSyntax Type { [DebuggerStepThrough] get; }
    public IdentifierName? ConstructorName { [DebuggerStepThrough] get; }
    public TextSpan? ConstructorNameSpan { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }

    public NewObjectExpressionSyntax(
        TextSpan span,
        ITypeNameSyntax typeSyntax,
        IdentifierName? constructorName,
        TextSpan? constructorNameSpan,
        IFixedList<IExpressionSyntax> arguments)
        : base(span)
    {
        Type = typeSyntax;
        Arguments = arguments;
        ConstructorName = constructorName;
        ConstructorNameSpan = constructorNameSpan;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        var name = ConstructorName is not null ? "." + ConstructorName : "";
        return $"new {Type}{name}({string.Join(", ", Arguments)})";
    }
}
