using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public interface IHasUsingNamespaces
{
    IFixedSet<string> UsingNamespaces { get; }
}
