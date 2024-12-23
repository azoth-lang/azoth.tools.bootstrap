# Types System Knowledge

## Type System Components
- Type Constructors: Build new types from existing ones
- Plain Types: Types before reference capabilities (like C#/Java)
- Decorated Types: Plain types with reference capability decorations
- Bare Types: Intermediate types with capabilities on arguments but not top-level

## Key Concepts
- Unbound Generic Type: Generic type without arguments (e.g. List[])
- Constructed Type: Generic type with at least one argument
- Open Type: Type involving type parameters
- Closed Type: Type without type parameters
- Bound Type: Non-generic type or generic type with all arguments specified

## Design Decisions
- No "I" prefix for interfaces used as traits
- Avoid `==` operator for types (use `.Equals()`)
- Reference capabilities are flow typed
