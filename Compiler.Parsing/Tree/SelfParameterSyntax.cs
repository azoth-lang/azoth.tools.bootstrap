using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SelfParameterSyntax : ParameterSyntax, ISelfParameterSyntax
{
    public bool IsMutableBinding => false;
    public IReferenceCapabilitySyntax Capability { get; }
    public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    public override IPromise<DataType> DataType { get; }
    public SelfParameterSyntax(TextSpan span, IReferenceCapabilitySyntax capability)
        : base(span, null)
    {
        Capability = capability;
        DataType = Symbol.Select(s => s.DataType);
    }

    public override string ToString()
        => Capability.Declared == DeclaredReferenceCapability.ReadOnly ? "self" : Capability + " self";
}
