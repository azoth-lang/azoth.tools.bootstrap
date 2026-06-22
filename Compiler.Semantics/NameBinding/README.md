# Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding

Name binding associates names to the nodes that define those names. In the Roslyn C# compiler,
binding occurs against symbols. But the Roslyn compiler doesn't expose a semantic tree. It already
needs complex lazy symbols with all important information attached. Azoth compiler symbols are
immutable combinations of name and type with reference only to their containing symbol. The tree
creates symbol nodes for symbols loaded from other packages.

Name binding uses the various collections of members on a definition/declaration. See the
[Compiler.Semantics Members Section](../README.md#members) for what those are. Binding of members is
done against denotations since binding must properly handle the types of inherited members
as-seen-from the type they are being accessed off of.

**TODO:** current code does not bind against denotations, but directly against members.
