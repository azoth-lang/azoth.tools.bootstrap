using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to pass through
/// the analysis.
/// </summary>
/// <remarks>Flow is based of of <see cref="BindingSymbol"/> because you can do things like change
/// the type of <s>self</s> if it started out isolated.</remarks>
public class FlowState
{
    private readonly SharingRelation sharing;
    private readonly ReferenceCapabilities capabilities;

    public FlowState(ReferenceCapabilities capabilities, SharingRelation sharing)
    {
        this.capabilities = capabilities;
        this.sharing = sharing;
    }

    public FlowState(ReferenceCapabilitiesSnapshot capabilities, SharingRelationSnapshot sharing)
    {
        this.capabilities = capabilities.MutableCopy();
        this.sharing = sharing.MutableCopy();
    }

    public FlowState()
    {
        capabilities = new ReferenceCapabilities();
        sharing = new SharingRelation();
    }

    public void Declare(BindingSymbol symbol)
    {
        capabilities.Declare(symbol);
        sharing.Declare(symbol);
    }

    public void Drop(BindingSymbol symbol) => sharing.Drop(symbol);

    public void DropAllLocalVariable() => sharing.DropAllLocalVariable();

    public void DropIsolatedParameters() => sharing.DropIsolatedParameters();

    public void Move(BindingSymbol symbol) => capabilities.Move(symbol);

    public void Freeze(BindingSymbol symbol) => capabilities.Freeze(symbol);

    // TODO maybe this happens as part of Union?
    public void Alias(BindingSymbol symbol) => capabilities.Alias(symbol);

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(BindingSymbol? symbol) => capabilities.CurrentType(symbol);

    public void Union(SharingVariable var1, SharingVariable var2) => sharing.Union(var1, var2);

    public void UnionResult(BindingSymbol symbol) => sharing.UnionResult(symbol);

    public void Split(SharingVariable variable) => sharing.Split(variable);

    public bool IsIsolated(SharingVariable variable) => sharing.IsIsolated(variable);

    public bool IsIsolatedExceptResult(SharingVariable variable) => sharing.IsIsolatedExceptResult(variable);
}
