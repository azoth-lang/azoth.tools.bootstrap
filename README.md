# Azoth Bootstrap Compiler

A bootstrap compiler for the [Azoth language](http://azoth-lang.org). That is, a compiler for a
subset of the language that will be used to write the Azoth compiler in Azoth. The compiler
currently runs as a tree interpreter.

## Project Status: Alpha Active

The compiler is under active development. It is in a very early stage, and there are likely issues
and limitations. APIs are subject to frequent breaking changes.

## Current Plan

The current plan is to get a basic object-oriented subset of the language working. The inspiration
for this is the feature set of Java 1.0. While Java 1.0 was very limiting, it demonstrates a viable
language for general purpose development. This subset of the language should provide experience with
the viability of developing with compile-time memory management. It will also provide a platform for
experimenting with how to best support language features like asynchronous programming, class
extension, closures, exceptions, and effect types.

The basic object-oriented subset is planned to include a minimal set of features to support
programming. The list below should be taken as a general guideline. Some very basic language
features have been omitted. For features that are listed, often only a very basic version of them
will be supported. At the end of the list are some features that may be necessary to include, but
will be omitted if possible.

The compiler will be simplified to include code only for these features. No attempt will be made to
structure code for features that aren't currently present. For example, function types will not be
used internally because function types aren't supported in this version of the language.
Additionally, the compiler will be multi-phase with errors in any phase preventing subsequent phases
from running. It is expected the phases will be lexing and parsing; name binding and type checking;
and borrow checking.

* [x] Stand-alone Functions - no generics
* [x] Simple Types - `int`, `uint`, `size`, `offset`
* [x] Simple Class Declarations - no base class, no generics (see below for supported members)
* [x] Fields - no initializers
* [x] Constructors
* [x] Basic Methods - no generics
* [x] Optional Types
* [x] Basic Control Flow - `if`, `while`, `loop` `break`, `next`
* [~] Conversions and Casts - `as`, `as!`, `as?` (not including unboxing)
* [x] Namespaces
* [ ] Strings
* [ ] `foreach n in 1..100` - basic loop iteration may need to be hard-coded because the final
  version will depend on features not yet available.
* [x] Basic `List[T]`
* [ ] Basic Traits? - no generics, classes directly implement like interfaces
* [ ] `foreach` in Iterator?
* [ ] `string` as primitive type
* [x] Use `get_` and `set_` instead of properties. (Avoids conflict with field names and allows for
  the exploration of the proper types for getters and setters.)

These features should allow a number of basic programs and katas to be implemented. Note that the
lack of generics will mean that future versions won't be backwards compatible with the standard
library of this version.

As this subset is developed, the plan will be to:

* Make the *Azoth source* code as correct as possible given all current ideas on the design of the
  language with the exception of what isn't possible due to missing features (for example,
  generics).
* Ensure any language features used are in the language reference.
* If needed, parts of what will be the standard library can be created as compiler intrinsics at
  first, but they should be replaced with correct standard library code when possible.

### Excluded Language Features

The following features will not be implemented by this compiler even though they are described in
the [language reference](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/book.md).

* [Loop Labels](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/loop-expressions.md#loop-labels)
* [Document Comment Validation](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/documentation-comments.md#documentation-comments)
* Full Type Inference: Variable types can be inferred from the type of their initializer only.

### Features Planned for After v1.0

A number of language features are planned for the future but not yet included in the language. For
quick reference, SOME of those features are:

* [Global and Package Qualifiers](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-qualifier.md)
* [Additional Types](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-types.md)
  * [128-bit Integer Types](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-types.md#128-bit-integer-types)
  * [128-bit Floating Point Type](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-types.md#128-bit-floating-point-type)
  * [Fixed Point Types](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-types.md#fixed-point-types)
  * [Decimal Types](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-types.md#decimal-types)
  * [Real Types](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-types.md#real-types)
* [Aliases](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-aliases.md)
* [Additional Expressions](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-expressions.md)
  * [Multiline String Literals](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-expressions.md#multiline-string-literals)
* [Operator Features](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-operators.md)
* [Compile-time Function Execution](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-ctfe.md)
* [Language-Oriented Programming](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/planned-lop.md)

The full list can be found in the [language reference](https://github.com/azoth-lang/azoth.language.reference/blob/master/src/book.md) planned features section.

## Cleanup Tasks

None Currently

## Conventions

* Unit tests are in projects named `Tests.Unit.*`. This way, it is not inconsistent when further
  namespaces are nested inside them. If `Tests` were at the end of the name, then many namespaces
  would have it at the end, while nested ones would have it in the middle. This also allows
  conformance and integration tests to be grouped with them by placing them all under the `Tests`
  namespace.
* Namespace hierarchies are kept fairly flat. This is to avoid issues of needing too many `using`
  statements and moving types between them. A namespace should contain all classes that represent
  the same "kind" of entity without much regard to sub-kinds. Originally, this was not the case.
  Types were separated into many sub-namespaces, but this ended up being more trouble than it was
  worth. In practice, one still just used the go to type functionality to find types.
* For many classes, there isn't a single string representation that makes the most sense. For these,
  a `ToILString()` and `ToSourceCodeString()` method have been added. Originally, the `ToString()`
  would throw `NotSupportedException`. However, there are cases with the .NET framework calls
  `ToString()` in order to create some exception message. As a result, throwing
  `NotSupportedException` causes problems. So instead, it should forward to `ToILString()` since
  that is the developer relevant string. Likewise, `ToILString()` is used as the debug
  representation.

## Line Count

A line count can be obtained from Visual Studio 2019 using the "Code Metrics" feature. An alternate
approximation can be found in other versions of Visual Studio by running "Find All" with regular
expressions across all `*.cs` files using the regex `^.*[^\s{}].*.$`. This matches all lines that
aren't blank or only curly braces. Note that this does match lines that are only comments and
includes a few generated code files.
