using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed class RuleNode
{
    public Symbol Defines { get; }
    public Symbol? Parent { get; }
    public IFixedSet<Symbol> Supertypes { get; }
    public IEnumerable<Symbol> Parents => Parent is null ? Supertypes : Supertypes.Append(Parent);
    public IFixedList<PropertyNode> Properties { get; }

    public RuleNode(
        Symbol defines,
        Symbol? parent,
        IEnumerable<Symbol> supertypes,
        IEnumerable<PropertyNode> properties)
    {
        Defines = defines;
        Parent = parent;
        Supertypes = supertypes.ToFixedSet();
        Properties = properties.ToFixedList();
        if (Properties.Select(p => p.Name).Distinct().Count() != Properties.Count)
            throw new ArgumentException($"Rule for {defines} contains duplicate property definitions");
    }

    public RuleNode WithDefaultParent(Symbol? defaultParent)
    {
        if (Parent is not null
            || defaultParent is null
            || Defines == defaultParent) return this;
        return new RuleNode(Defines, defaultParent, Supertypes, Properties);
    }

    public override string ToString()
    {
        var parent = Parent is null ? "" : $": {Parent}";
        var supertypes = Supertypes.Count == 0 ? "" : " <: " + string.Join(", ", Supertypes);
        var properties = Properties.Count == 0 ? "" : " = " + string.Join(", ", Properties);
        return $"{Defines}{parent}{supertypes}{properties};";
    }
}
