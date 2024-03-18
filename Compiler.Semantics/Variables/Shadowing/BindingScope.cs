using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Shadowing;

public abstract class BindingScope
{
    public bool Lookup(SimpleName name, [NotNullWhen(true)] out VariableBinding? binding)
        => LookupWithoutNumber(name, out binding);

    protected abstract bool LookupWithoutNumber(SimpleName name, [NotNullWhen(true)] out VariableBinding? binding);

    /// <summary>
    /// Indicates that some nested scope declared a variable binding.
    /// </summary>
    protected internal abstract void NestedBindingDeclared(VariableBinding binding);
}
