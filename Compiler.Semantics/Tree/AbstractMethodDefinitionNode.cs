using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AbstractMethodDefinitionNode : MethodDefinitionNode, IAbstractMethodDefinitionNode
{
    public override IAbstractMethodDefinitionSyntax Syntax { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ValueAttribute<ObjectType> containingDeclaredType;
    public ObjectType ContainingDeclaredType
        => containingDeclaredType.TryGetValue(out var value) ? value
            : containingDeclaredType.GetValue(InheritedContainingDeclaredType);

    public AbstractMethodDefinitionNode(
        IAbstractMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
    }

    protected override ObjectType InheritedContainingDeclaredType()
        => (ObjectType)base.InheritedContainingDeclaredType();

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeModifiersAspect.AbstractMethodDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
