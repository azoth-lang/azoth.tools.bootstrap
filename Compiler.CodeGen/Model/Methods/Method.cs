using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract record Method
{
    public NonVoidType FromType { get; }
    public abstract required IFixedList<Parameter> AdditionalParameters { get; init; }

    private protected Method(NonVoidType fromType)
    {
        FromType = fromType;
    }

    /// <summary>
    /// Get the methods this method calls.
    /// </summary>
    /// <remarks>This is a method instead of a property because the method object are recreated
    /// after this is called. So it only makes sense for the method objects as they exist at the
    /// time.</remarks>
    public abstract IEnumerable<Method> GetMethodsCalled();
}
