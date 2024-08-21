using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class AspectSyntax
{
    public string Namespace { get; }
    public string Name { get; }
    public IFixedSet<TypeDeclarationSyntax> TypeDeclarations { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedSet<InheritedAttributeSupertypeSyntax> AttributeSupertypes { get; }
    public IFixedList<AspectAttributeSyntax> Attributes { get; }
    public IFixedList<EquationSyntax> Equations { get; }

    public AspectSyntax(
        string @namespace,
        string name,
        IEnumerable<string> usingNamespaces,
        IEnumerable<TypeDeclarationSyntax> typeDeclarations,
        IEnumerable<InheritedAttributeSupertypeSyntax> attributeSupertypes,
        IEnumerable<AspectAttributeSyntax> attributes,
        IEnumerable<EquationSyntax> equations)
    {
        Namespace = @namespace;
        Name = name;
        TypeDeclarations = typeDeclarations.ToFixedSet();
        UsingNamespaces = usingNamespaces.ToFixedSet();
        AttributeSupertypes = attributeSupertypes.ToFixedSet();
        Attributes = attributes.ToFixedList();
        Equations = equations.ToFixedList();
    }
}
