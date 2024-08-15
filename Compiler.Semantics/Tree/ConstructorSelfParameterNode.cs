using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
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
    private ValueAttribute<CapabilityType> bindingType;
    public override CapabilityType BindingType
        => bindingType.TryGetValue(out var value) ? value
            : bindingType.GetValue(this, TypeMemberDeclarationsAspect.ConstructorSelfParameter_BindingType);

    public ConstructorSelfParameterNode(IConstructorSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        TypeMemberDeclarationsAspect.ConstructorSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
