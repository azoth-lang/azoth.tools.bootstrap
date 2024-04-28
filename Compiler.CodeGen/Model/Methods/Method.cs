using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract class Method
{
    public abstract IFixedList<Parameter> AdditionalParameters { get; }

    private protected Method() { }

    /// <summary>
    /// Get the methods this method calls.
    /// </summary>
    /// <remarks>This is a method instead of a property because the method object are recreated
    /// after this is called. So it only makes sense for the method objects as they exist at the
    /// time.</remarks>
    public abstract IEnumerable<Method> GetMethodsCalled();
}
