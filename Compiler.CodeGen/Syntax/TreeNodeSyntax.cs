using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class TreeNodeSyntax
{
    public SymbolSyntax Defines { get; }
    public IFixedSet<SymbolSyntax> Supertypes { get; }
    public IFixedList<PropertySyntax> DeclaredProperties { get; }

    public TreeNodeSyntax(
        SymbolSyntax defines,
        IEnumerable<SymbolSyntax> supertypes,
        IEnumerable<PropertySyntax> declaredProperties)
    {
        Defines = defines;
        var supertypesList = supertypes.ToFixedList();
        if (supertypesList.Duplicates().Any())
            throw new ArgumentException($"Node {defines} contains duplicate supertype definitions.");
        Supertypes = supertypesList.ToFixedSet();
        DeclaredProperties = declaredProperties.ToFixedList();
        if (DeclaredProperties.Select(p => p.Name).Distinct().Count() != DeclaredProperties.Count)
            throw new ArgumentException($"Node {defines} contains duplicate property definitions.");
    }

    public override string ToString()
    {
        var supertypes = Supertypes.Count == 0 ? "" : " <: " + string.Join(", ", Supertypes);
        var properties = DeclaredProperties.Count == 0 ? "" : " = " + string.Join(", ", DeclaredProperties);
        return $"{Defines}{supertypes}{properties};";
    }
}
