using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract record Method
{
    public Pass Pass { get; }
    public NonVoidType FromType { get; }
    public abstract required IFixedList<Parameter> AdditionalParameters { get; init; }

    private protected Method(Pass pass, NonVoidType fromType)
    {
        FromType = fromType;
        Pass = pass;
    }

    /// <summary>
    /// Get the methods this method calls.
    /// </summary>
    /// <remarks>This is a method instead of a property because the method object are recreated
    /// after this is called. So it only makes sense for the method objects as they exist at the
    /// time.</remarks>
    public abstract IEnumerable<Method> GetMethodsCalled();

    /// <summary>
    /// Get the create method that should be called for the given rule.
    /// </summary>
    protected Method GetCreateMethodCalled(Rule toRule)
    {
        Requires.That(nameof(toRule), toRule.Grammar == Pass.ToLanguage?.Grammar, "Must be in the 'To' language");
        return !toRule.IsTerminal || toRule.DifferentChildProperties.Any()
            ? Pass.AdvancedCreateMethods.Single(m => m.ToRule == toRule)
            : Pass.SimpleCreateMethods.Single(m => m.ToRule == toRule);
    }
}
