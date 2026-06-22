# Azoth.Tools.Bootstrap.Compiler.Semantics

## Members

There are quite a few different flavors or members that need to be organized in various collections.
First, members can be instance members or associated members. Note that initializers are considered
associated members. This split applies to each of the other categories and so isn't listed
separately. The collections of definition/declaration members available on a type
definition/declaration are:

* Source Associated/Instance Members: Members declared in the explicitly in the source code. Here,
  "source" has a double meaning as the members in "source" code and as the members from which
  everything else is derived.
* Synthesized Instance/Associated Members: Members synthesized by the compiler based on other
  declarations. For example, implicit initializers and drop methods.
* Declared Instance/Associated Members: the union of the source and synthesized members. These are
  the declared members because the the source members are explicitly declared and the synthesized
  members are implicitly declared.
* Nested Members: For namespaces, the nested members includes not only the members of this
  namespace, but the nested members of all namespaces inside that namespace. This is useful for name
  lookup since name resolution searches the nested members.

In addition to those, there are denotation members. The collections of denotations members available
on a type definition/declaration are:

* Base/supertype Instance/Associated Member Denotations: denotations for the base or supertype
  members as-seen-from the current type without consideration for which are superseded.
* Inherited Instance/Associated Member Denotations: denotations for members that are inherited to
  this type from the base and supertype because they were not superseded.
* Instance/Associated Member Denotations: denotations for all members on a type. This includes the
  inherited ones but also the denotations for all declared members on the type.

Individual member declarations have superseded member denotations which given the base/supertype
denotations that are overridden or hidden by the current member.

### Outdated Terminology

The term "inclusive members" was previously used to refer to the members of a type along with any
inherited members. However, that was before the concept of denotation was introduced. The inclusive
members are not the right way to model the problem because an inherited member needs the context of
the type it is being accessed from.

## Glossary

* Declaration: a declaration of a type or member. A declaration introduces a name into a scope and
  tells the compiler its kind and type, without necessarily providing enough information to allocate
  storage or emit code for it. This could be a definition in source code, but it could also be a
  symbol from a dependency that declares it.
* Definition: the actual definition of a type or member in source code including the body. A
  definition provides the complete entity: it allocates the storage, supplies the function body, or
  specifies the full layout of a type.
* Denotation: a declaration as as-seen-from a type. That is, it pairs a member declaration with the
  type it is being accessed through to provide member types with substitution applied. This term is
  borrowing from formal/denotational semantics, where the denotation of an expression is the meaning
  or referent it's mapped to under an interpretation. The salient property in that sense is that the
  referent is *relative to a context*. This term is used in a similar way in Scala 3.
* Superseded: a member is superseded by another if it is overridden or hidden by that member.

## History

There have been several approaches to the semantic analysis portion of the compiler.

### Original Approach

At first, it was thought that this compiler should be designed to plan for reimplementation in Azoth. Doing so imposed a number of constraints:

1. Avoid Reference Cycles - borrow checking and reference counting in Azoth make reference cycles problematic. In particular, there was concern about links across the tree. To avoid those, the plan had been to store them as name or data type objects that could be used to look up the relevant nodes in a symbol tree.
2. Incremental Compilation - prevented the use of references to parent nodes even though these might be ok in Azoth because it was expected nodes would be shared between different versions of the tree. Also, it pushed for immutable data structures for all phases of the compiler. It was not planned that this compiler would actually implement incremental compilation though.
3. Arbitrary Compile Time Code Execution - means that there could be arbitrarily complex connections between which parts of the semantic tree must be evaluated before other parts. For example, the ability to declare the return type of a function to be the result of a meta-function and the ability of pure functions to be used in constant and generic argument contexts means that some functions will have to be fully analyzed and interpreted before other functions can even be typed.
4. Parallel Compilation - it must be possible to break the work into chunks while not producing undo contention.
5. Strong Typing - to avoid bugs involving missing values or values of incorrect types.

Trying to support all of these while creating a compiler that could be rapidly developed and changed was challenging. C# seemed to make everything very verbose. A series of approaches were tried. Eventually a system allowing for a fairly direct expression of attribute grammars over the syntax tree was adopted. However, this makes strong typing challenging. Worse than that, because everything is computed on demand it makes order of execution and debugging very confusing. To simplify the implementation of the borrow checker an intermediate language (IL) was introduced. This was helpful, however, it was added as a later phase to semantic analysis which then became an issue because it couldn't be used for compile time code execution because it could only be generated after all semantic analysis was complete.

### Current Plan

Of the above constraints, all but arbitrary compile time code execution and strong typing have been dropped. It is hoped this will make development quicker, easier and more testable. To make things clear and easy, the plan to to have a series of clearly defined steps. Of the below list, only steps 2 through 4 are part of semantic the semantic analysis in this project. Steps 1 and 5 are included for context.

1. Lexing and Parsing produce a concrete syntax tree. Each compilation unit can be done in parallel.
2. Build a simplified tree of declarations and members for the package. (Consider including simplified statements and expressions too)
3. Build lexical scopes (i.e. name tables) for name resolution. Each compilation unit can be done in parallel.
4. Analyze the semantics. This must mix name binding, type checking, IL generation and compile time code execution because they are interdependent. To simplify this as much as possible, this will be single threaded and will be free to mutate the tree as needed. All mutator method/properties will be `internal` to other phases can't mutate the tree. The `Lazy<T>` type may or may not be used to make cycle detection easier.
5. Emit C code from the generated IL.

#### Semantic/IL Tree

The current plan is that the tree generated for the semantic analysis and IL generation would be something roughly consistent with what could be used in an IL representation of an Azoth package. However, it may include additional data or optional data structures that wouldn't be used if parsing and generating IL. In order to try to keep this structure as simple as possible, it is planned the following abstractions relative to the concrete syntax trees will be made:

* Unify partial classes
* Don't represent namespaces hierarchically, each declaration is fully qualified
* Don't include parenthesized expressions
* Unify the different kinds of blocks
* All loops as `loop`?
