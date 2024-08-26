using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PropertyNameNode : AmbiguousNameExpressionNode, IPropertyNameNode
{
    protected override bool MayHaveRewrite => true;

    public override IMemberAccessExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public StandardName PropertyName { get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }

    public PropertyNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
    }

    protected override IChildNode Rewrite()
        => BindingAmbiguousNamesAspect.PropertyName_Rewrite(this) ?? base.Rewrite();
}
