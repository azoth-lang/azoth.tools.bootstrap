# Azoth.Tools.Bootstrap.Compiler.Symbols

A symbol is the pairing of a full name with a type and other data about the symbol.

Symbols do not directly reference their children. Instead they only reference their containing
symbols. That is necessary to avoid circular dependencies when constructing immutable symbols. The
Roslyn C# compiler provides `GetMembers()` methods in addition to `ContainingSymbol`. However, it
does lazy loading on symbol properties. To keep symbols symbols and avoid coupling them to the
semantic tree, the Azoth compiler doesn't do that. Instead, symbols are loading into an
`ISymbolTree` which can be used to find the members of any given symbol.

It may make sense to eliminate these symbol object eventually. A symbol in Azoth is a logical entity
that exists primarily in serialized form in packages. There are symbols for declarations with
implementations associated. There are symbols that reference declared symbols in other packages. So
the concept of a symbol is important, but it isn't clear if these objects are valuable for that.
