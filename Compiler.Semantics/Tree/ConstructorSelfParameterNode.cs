using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ConstructorSelfParameterNode : SelfParameterNode, IConstructorSelfParameterNode
{
    public override IConstructorSelfParameterSyntax Syntax { get; }
    public new ObjectType ContainingDeclaredType => (ObjectType)base.ContainingDeclaredType;
    public ICapabilityNode Capability { get; }
    public override IdentifierName? Name => Syntax.Name;
    private ValueAttribute<ReferenceType> type;
    public override ReferenceType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.ConstructorSelfParameter_Type);

    public ConstructorSelfParameterNode(IConstructorSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeMemberDeclarationsAspect.ConstructorSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
