# Azoth.Tools.Bootstrap.Compiler.Semantics

Contents:

* [Members](#members)
  * [Outdated Terminology](#outdated-terminology)
* [Glossary](#glossary)

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
