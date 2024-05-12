using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerSelfParameterNode : SelfParameterNode, IInitializerSelfParameterNode
{
    public override IInitializerSelfParameterSyntax Syntax { get; }
    public new StructType ContainingDeclaredType => (StructType)base.ContainingDeclaredType;
    public ICapabilityNode Capability { get; }
    public override IdentifierName? Name => Syntax.Name;
    private ValueAttribute<ValueType> type;
    public override ValueType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, InvocableDeclarationsAspect.InitializerSelfParameter_Type);

    public InitializerSelfParameterNode(IInitializerSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        InvocableDeclarationsAspect.InitializerSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
