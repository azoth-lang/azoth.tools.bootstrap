using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to pass through
/// the analysis.
/// </summary>
/// <remarks>Flow is based of of <see cref="BindingSymbol"/> because you can do things like change
/// the type of <s>self</s> if it started out isolated.</remarks>
public sealed class FlowState
{
    private readonly ReferenceCapabilities capabilities;
    private readonly SharingRelation sharing;

    public FlowState(ReferenceCapabilitiesSnapshot capabilities, SharingRelationSnapshot sharing)
        : this(capabilities.MutableCopy(), sharing.MutableCopy())
    {
    }

    public FlowState() : this(new ReferenceCapabilities(), new SharingRelation())
    {
    }

    private FlowState(ReferenceCapabilities capabilities, SharingRelation sharing)
    {
        this.capabilities = capabilities;
        this.sharing = sharing;
    }

    public ResultVariable CurrentResult => sharing.CurrentResult;

    public FlowState Copy() => new(capabilities.Copy(), sharing.Copy());

    public void Declare(BindingSymbol symbol)
    {
        capabilities.Declare(symbol);
        sharing.Declare(symbol);
    }

    public SharingVariable NewResult() => sharing.NewResult();

    public void Drop(BindingSymbol symbol) => sharing.Drop(symbol);

    public void DropAllLocalVariables() => sharing.DropAllLocalVariables();

    public void DropIsolatedParameters() => sharing.DropIsolatedParameters();

    public void Move(BindingSymbol symbol) => capabilities.Move(symbol);

    public void Freeze(BindingSymbol symbol)
    {
        capabilities.Freeze(symbol);
        // Constants aren't tracked, this symbol is `const` now
        sharing.Drop(symbol);
    }

    // TODO maybe this happens as part of Union?
    public void Alias(BindingSymbol symbol) => capabilities.Alias(symbol);

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(BindingSymbol? symbol) => capabilities.CurrentType(symbol);

    public void UnionWithCurrentResult(SharingVariable var) => sharing.UnionWithCurrentResult(var);

    public void UnionWithCurrentResultAndDrop(ResultVariable variable)
        => sharing.UnionWithCurrentResultAndDrop(variable);

    public void DropCurrentResult() => sharing.DropCurrentResult();

    public void SplitCurrentResult() => sharing.SplitCurrentResult();

    public bool CurrentResultIsIsolated() => sharing.CurrentResultIsolated();
    public bool IsIsolated(SharingVariable variable) => sharing.IsIsolated(variable);

    public bool IsIsolatedExceptCurrentResult(SharingVariable variable)
        => sharing.IsIsolatedExceptCurrentResult(variable);
}
