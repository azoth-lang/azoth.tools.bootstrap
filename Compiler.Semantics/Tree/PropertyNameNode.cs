using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PropertyNameNode : AmbiguousNameExpressionNode, IPropertyNameNode
{
    protected override bool MayHaveRewrite => true;

    public override IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName PropertyName { get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }

    public PropertyNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
    }

    protected override IChildNode? Rewrite()
        => BindingAmbiguousNamesAspect.PropertyName_Rewrite(this);
}
