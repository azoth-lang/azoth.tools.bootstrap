using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GetterInvocationExpressionNode : ExpressionNode, IGetterInvocationExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName PropertyName { get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.GetterInvocationExpression_Antetype);

    public GetterInvocationExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        IGetterMethodDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
    }
}
