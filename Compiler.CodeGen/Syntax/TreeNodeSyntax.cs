using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class TreeNodeSyntax
{
    public bool IsTemp { get; }
    public bool? IsAbstract { get; }
    public SymbolSyntax Defines { get; }
    public IFixedSet<SymbolSyntax> Supertypes { get; }
    public IFixedList<TreeAttributeSyntax> DeclaredAttributes { get; }

    public TreeNodeSyntax(
        bool isTemp,
        bool? isAbstract,
        SymbolSyntax defines,
        IEnumerable<SymbolSyntax> supertypes,
        IEnumerable<TreeAttributeSyntax> declaredAttributes)
    {
        IsTemp = isTemp;
        IsAbstract = isAbstract;
        Defines = defines;
        var supertypesList = supertypes.ToFixedList();
        if (supertypesList.Duplicates().Any())
            throw new ArgumentException($"Node {defines} contains duplicate supertype definitions.");
        Supertypes = supertypesList.ToFixedSet();
        DeclaredAttributes = declaredAttributes.ToFixedList();
        if (DeclaredAttributes.Select(p => p.Name).Distinct().Count() != DeclaredAttributes.Count)
            throw new ArgumentException($"Node {defines} contains duplicate attribute definitions.");
    }

    public override string ToString()
    {
        var supertypes = Supertypes.Count == 0 ? "" : " <: " + string.Join(", ", Supertypes);
        var attributes = DeclaredAttributes.Count == 0 ? "" : " = " + string.Join(", ", DeclaredAttributes);
        return $"{Defines}{supertypes}{attributes};";
    }
}
