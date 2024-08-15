using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericNameExpressionNode : AmbiguousNameExpressionNode, IGenericNameExpressionNode
{
    public override IGenericNameExpressionSyntax Syntax { get; }
    public GenericName Name => Syntax.Name;
    public IFixedList<ITypeNode> TypeArguments { get; }
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                InheritedContainingLexicalScope, ReferenceEqualityComparer.Instance);
    private ValueAttribute<IFixedList<IDeclarationNode>> referencedDeclarations;
    public IFixedList<IDeclarationNode> ReferencedDeclarations
        => referencedDeclarations.TryGetValue(out var value) ? value
            : referencedDeclarations.GetValue(this, BindingAmbiguousNamesAspect.StandardNameExpression_ReferencedDeclarations);

    public GenericNameExpressionNode(IGenericNameExpressionSyntax syntax, IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}
