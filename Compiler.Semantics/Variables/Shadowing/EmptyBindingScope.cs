using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Shadowing;

public class EmptyBindingScope : BindingScope
{
    #region Singleton
    public static readonly BindingScope Instance = new EmptyBindingScope();

    private EmptyBindingScope() { }
    #endregion

    protected override bool LookupWithoutNumber(IdentifierName name, [NotNullWhen(true)] out VariableBinding? binding)
    {
        binding = null;
        return false;
    }

    protected internal override void NestedBindingDeclared(VariableBinding binding)
    {
        // Empty scope has no bindings, so nested bindings don't matter
    }
}
