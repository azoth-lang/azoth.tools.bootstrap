# Azoth.Tools.Bootstrap.Compiler.Types

## Type Constructors

In type theory terms, a *type constructor* is a construct that builds new types from old one. For
example, a generic type definition constructs a family of related types (e.g. `List[T]` constructs
`List[int]`, `List[Shape]`, etc.). Basic types are constructed using *nullary* type constructors.

## Plain Types

Plain types are types as they would exist in Azoth before reference capabilities. They are similar
to how types work in C# or Java. Plain types were introduced because of a circular dependency where
overload resolution depended on reference capability flow types, but that flow typing depended on
which method overload was selected. Since reference capabilities are flow typed, where the type and
validity of each expression depends on the one before and in the case of loops can even depend on
subsequent ones, it would have been necessary to consider the resolution of multiple calls
simultaneously. That is `a(...) + b(...)` would, in some cases, require evaluation every possible
pair of overloads for both `a` and `b` to determine which worked together. Instead, overloading on
reference capability has been removed from the language and overload resolution is determined by the
plain type. Once overload resolution and name binding are fixed by plain types, then the normal
reference capability based types can be determined and any errors resulting from capability
violation reported.

## Decorated Types

Decorated types are plain types with the reference capability decorations. These correspond to the
surface level types in an Azoth program. As such, they are named just *`Types` rather than
*`DecoratedTypes`.

## Bare Types

During type computation, it is necessary to have as an intermediate step, a type that has reference
capabilities on type arguments, but does not *yet* have a reference capability at the top-level.
These are referred to as *bare types*. They really aren't a valid or complete system of types.
Rather an incomplete subset of the types. Namely, those that could have a capability applied to
them.

## Conventions

This project doesn't prefix interfaces used as traits with "`I`". This is because the
class/interface hierarchy for types is so complex and really needs traits. As such it is often the
case that base classes should be equivalent to traits and that it isn't always obvious which should
be used where.

Also, this project does not declare the `==` operator for types because it cannot be declared on
interfaces. When `==` was used, there had been bugs where `==` was used to compare types when
`.Equals()` was needed because they happened to be accessed through interfaces. This inconsistency
was confusing and error prone.

The word "Named" is used instead of "Nominal" because it has fewer other meanings that could confuse
the meaning of class names.

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
