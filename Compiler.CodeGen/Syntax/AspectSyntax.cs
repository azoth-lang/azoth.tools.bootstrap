using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class AspectSyntax
{
    public string Namespace { get; }
    public string Name { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<AttributeSyntax> Attributes { get; }
    public IFixedList<EquationSyntax> Equations { get; }

    public AspectSyntax(
        string @namespace,
        string name,
        IEnumerable<string> usingNamespaces,
        IEnumerable<AttributeSyntax> attributes,
        IEnumerable<EquationSyntax> equations)
    {
        Namespace = @namespace;
        Name = name;
        UsingNamespaces = usingNamespaces.ToFixedSet();
        Attributes = attributes.ToFixedList();
        Equations = equations.ToFixedList();
    }
}
