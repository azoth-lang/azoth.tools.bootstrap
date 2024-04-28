using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract class Method
{
    public abstract IFixedList<Parameter> AdditionalParameters { get; }
    public abstract IFixedSet<Method> CallsMethods { get; }

    private protected Method() { }
}
