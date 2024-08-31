using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

/// <summary>
/// An attribute that is part of the tree definition (as opposed to one that is part of an aspect).
/// </summary>
[Closed(typeof(PropertySyntax), typeof(PlaceholderSyntax))]
public abstract class TreeAttributeSyntax : AttributeSyntax
{
    protected TreeAttributeSyntax(string name)
        : base(name) { }
}
