using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterInvocationExpressionNode : ExpressionNode, ISetterInvocationExpressionNode
{
    public override IAssignmentExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName PropertyName { get; }
    private Child<IAmbiguousExpressionNode> value;
    public IAmbiguousExpressionNode Value => value.Value;
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.SetterInvocationExpression_Antetype);

    public SetterInvocationExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IAmbiguousExpressionNode value,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        ISetterMethodDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        PropertyName = propertyName;
        this.value = Child.Create(this, value);
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
    }
}