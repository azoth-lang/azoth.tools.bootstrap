using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
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
    private ValueAttribute<ContextualizedOverload<IGetterMethodDeclarationNode>?> contextualizedOverload;
    public ContextualizedOverload<IGetterMethodDeclarationNode>? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.GetterInvocationExpression_ContextualizedOverload);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.GetterInvocationExpression_Type, ref type);

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
