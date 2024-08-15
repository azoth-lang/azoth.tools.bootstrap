using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class RuleSyntax
{
    public SymbolSyntax Defines { get; }
    public SymbolSyntax? Parent { get; }
    public IFixedSet<SymbolSyntax> Supertypes { get; }
    public IFixedList<SymbolSyntax> Parents { get; }
    public IFixedList<PropertySyntax> DeclaredProperties { get; }

    public RuleSyntax(
        SymbolSyntax defines,
        SymbolSyntax? parent,
        IEnumerable<SymbolSyntax> supertypes,
        IEnumerable<PropertySyntax> declaredProperties)
    {
        Defines = defines;
        Parent = parent;
        Supertypes = supertypes.ToFixedSet();
        DeclaredProperties = declaredProperties.ToFixedList();
        Parents = (Parent is null ? Supertypes : Supertypes.Prepend(Parent)).ToFixedList();
        if (Parents.Distinct().Count() != Parents.Count)
            throw new ArgumentException($"Rule for {defines} contains duplicate parent definitions");
        if (DeclaredProperties.Select(p => p.Name).Distinct().Count() != DeclaredProperties.Count)
            throw new ArgumentException($"Rule for {defines} contains duplicate property definitions");
    }

    public RuleSyntax WithDefaultParent(SymbolSyntax? defaultParent)
    {
        if (Parent is not null
            || defaultParent is null
            || Defines == defaultParent) return this;
        return new RuleSyntax(Defines, defaultParent, Supertypes, DeclaredProperties);
    }

    public override string ToString()
    {
        var parent = Parent is null ? "" : $": {Parent}";
        var supertypes = Supertypes.Count == 0 ? "" : " <: " + string.Join(", ", Supertypes);
        var properties = DeclaredProperties.Count == 0 ? "" : " = " + string.Join(", ", DeclaredProperties);
        return $"{Defines}{parent}{supertypes}{properties};";
    }
}
