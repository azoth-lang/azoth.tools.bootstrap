using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class AspectSyntax
{
    public string Namespace { get; }
    public string Name { get; }
    public IFixedSet<string> UsingNamespaces { get; }

    public AspectSyntax(string @namespace, string name, IEnumerable<string> usingNamespaces)
    {
        Namespace = @namespace;
        Name = name;
        UsingNamespaces = usingNamespaces.ToFixedSet();
    }
}
