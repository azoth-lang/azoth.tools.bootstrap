# Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols

This package manages the following symbol related things in the semantic tree:

1. The symbol defined by or associated to any given tree node. These are exposed through the
   `Symbol` or `Symbols` properties.
2. The symbol of the containing node/scope. These are exposed through the `ContainingSymbol()`
   method.
3. The nodes generated from symbols loaded for package references. These provide a way for binding
   to occur against those external symbols.

## Obsolete

`ReferencedSymbol` was part of an obsolete approach to name binding. Instead, use
`ReferencedDeclaration`, `ReferencedField`, etc.
