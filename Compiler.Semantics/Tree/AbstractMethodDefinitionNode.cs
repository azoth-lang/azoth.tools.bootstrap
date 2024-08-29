using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AbstractMethodDefinitionNode : MethodDefinitionNode, IAbstractMethodDefinitionNode
{
    public override IAbstractMethodDefinitionSyntax Syntax { get; }
    public override MethodKind Kind => MethodKind.Standard;
    public override IBodyNode? Body => null;
    public int Arity => Parameters.Count;
    public FunctionType MethodGroupType => Symbol.MethodGroupType;
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                Inherited_ContainingDeclaredType);

    public AbstractMethodDefinitionNode(
        IAbstractMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        TypeModifiersAspect.AbstractMethodDeclaration_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }
}
