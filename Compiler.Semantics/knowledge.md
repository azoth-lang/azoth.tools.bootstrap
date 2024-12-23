# Semantics Knowledge

## Current Approach
- Single-threaded analysis
- Free to mutate tree as needed
- Internal mutator methods/properties
- May use Lazy<T> for cycle detection

## Processing Steps
1. Build simplified declaration tree
2. Build lexical scopes for name resolution
3. Mix name binding, type checking, IL generation
4. Handle compile-time code execution

## Semantic/IL Tree Abstractions
- Unified partial classes
- Fully qualified declarations
- No parenthesized expressions
- Unified blocks
- Simplified loop structures
