using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerSelfParameterNode : SelfParameterNode, IInitializerSelfParameterNode
{
    public override IInitializerSelfParameterSyntax Syntax { get; }
    public new StructType ContainingDeclaredType => (StructType)base.ContainingDeclaredType;
    public ICapabilityNode Capability { get; }
    public override IdentifierName? Name => Syntax.Name;
    private ValueAttribute<CapabilityType> bindingType;
    public override CapabilityType BindingType
        => bindingType.TryGetValue(out var value) ? value
            : bindingType.GetValue(this, TypeMemberDeclarationsAspect.InitializerSelfParameter_BindingType);

    public InitializerSelfParameterNode(IInitializerSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeMemberDeclarationsAspect.InitializerSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
