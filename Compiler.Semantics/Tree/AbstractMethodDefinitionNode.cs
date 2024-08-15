using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AbstractMethodDefinitionNode : MethodDefinitionNode, IAbstractMethodDefinitionNode
{
    public override IAbstractMethodDefinitionSyntax Syntax { get; }
    public override IBodyNode? Body => null;
    public int Arity => Parameters.Count;
    public FunctionType MethodGroupType => Symbol.MethodGroupType;
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ObjectType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public ObjectType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                InheritedContainingDeclaredType);

    public AbstractMethodDefinitionNode(
        IAbstractMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
    }

    /// <remarks>Overridden to more specific return type so it can be used as a method group in the
    /// <see cref="ContainingDeclaredType"/> property.</remarks>
    protected override ObjectType InheritedContainingDeclaredType(IInheritanceContext ctx)
        => (ObjectType)base.InheritedContainingDeclaredType(ctx);

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeModifiersAspect.AbstractMethodDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
