# Antetypes

Antetypes are types as they would exist in Azoth before reference capabilities. They are similar to
how types work in C# or Java. Antetypes were introduced because of a circular dependency where
overload resolution depended on reference capability flow types, but that flow typing depended on
which method overload was selected. Since reference capabilities are flow typed, where the type and
validity of each expression depends on the one before and in the case of loops can even depend on
subsequent ones, it would have been necessary to consider the resolution of multiple calls
simultaneously. That is `a(...) + b(...)` would, in some cases, require evaluation every possible
pair of overloads for both `a` and `b` to determine which worked together. Instead, overloading on
reference capability has been removed from the language and overload resolution is determined by the
antetypes. Once overload resolution and name binding are fixed by antetypes, then the normal
reference capability based types can be determined and any errors resulting from capability
violation reported.

## Definitions

*Unbound Generic Type*

A generic type without any type arguments (e.g. `List[]` or `Dictionary[,]`).

*Constructed Type*

A generic type that includes at least one type argument is called a constructed type (e.g.
`List[int]`, `List[List[]]`, or `Dictionary[]`)

*Open Type*:

An open type is a type that involves type parameters (e.g. `TData` or `List[TTData]` where `TData`
is a generic type parameter in scope).

*Closed Type*

A closed type is a type that is not an open type.

*Unbound Type*

A non-generic type or an unbound generic type.

*Bound Type*

A non-generic type or a generic type with all type arguments fully specified. (Note: this is
different from C# which includes all constructed types i.e. types with only some type arguments
specified.) Non-generic types are considered to be both bound and unbound.

*Declared Type*

A declared type *isn't actually a type*. Instead, it is the template or kind that types are based
on. For generic types, the declared type has the type parameters and forms a template for creating
types. For non-generic types, no arguments are needed to construct the type. As such, the declared
type and the type can be used interchangeably.
