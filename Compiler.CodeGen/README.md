# Azoth.Tools.Bootstrap.Compiler.CodeGen

## Tree File Syntax

| Declaration                                            | Meaning |
| ------------------------------------------------------ | ------- |
| `◊namespace` *DottedNamespaceName*`;`                  |         |
| `◊root-supertype` *Node*`;`                            |         |
| `◊prefix` *Text*`;`                                    |         |
| `◊suffix` *Text*`;`                                    |         |
| `◊gen-classes` (`true`\|`false`)`;`                    |         |
| `◊simplified-tree` (`true`\|`false`)`;`                |         |
| `◊class-prefix` *Text*`;`                              |         |
| `◊class-suffix` *Text*`;`                              |         |
| `◊using` *DottedNamespaceName*`;`                      |         |
| *Node* `=` *PropertyExp**`;`                           |         |
| *Node* `<:` *Node* (`,` *Node*)* `=` *PropertyExp**`;` |         |

| Property Expression | Meaning                                                        |
| ------------------- | -------------------------------------------------------------- |
| *Name*              | Property with the given name of a node type with the same name |
| *Name*`*`           | List with given name and node type                             |
| *Name*`*?`          | Optional list with given name and node type                    |
| `{`*Name*`}`        | Set with given name and node type                              |
| `{`*Name*`}?`       | Optional set with given name and node type                     |
| *Name*`:`*TypeExp*  | Property with the given name and type (see Type Expressions)   |
| `/`*Name*`/`        | A placeholder for controlling the order of a attributes        |

| Type Expression      | Meaning               |
| -------------------- | --------------------- |
| *Node*               | A node type           |
| `` ` ``*Type*`` ` `` | A C# type             |
| *BasicType*`*`       | List of type          |
| *BasicType*`*?`      | Optional list of type |
| `{`*BasicType*`}`    | Set of type           |
| `{`*BasicType*`}?`   | Optional set of type  |

### The Default Node

`<default>`

## Aspect File Syntax

| Declaration                           | Meaning |
| ------------------------------------- | ------- |
| `◊namespace` *DottedNamespaceName*`;` |         |
| `◊name` *Node*`;`                     |         |
| `◊using` *DottedNamespaceName*`;`     |         |

### Intertype Declared Members

**TODO:** There is a strange overlap between this and synthesized attributes that are methods. Maybe
these should be combined/simplified somehow? Remember that there are also inherited attributes that
are methods.


| Declaration                                                            | Meaning                                                                    |
| ---------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| `+` *Node*`.`*Attribute*`(`*Params*`):` *Type*`;`                      | Intertype method attribute, always computed                                |
| `+` *Node*`.`*Attribute*`(`*Params*`):` *Type* `=>` *Expression*`;`    | Intertype method attribute with default inline expression, always computed |
| `=` *Node*`.`*Attribute*`(`*Params*`):` *Type* (`=>` *Expression*)?`;` | Intertype method equation                                                  |

### Type Declarations

For inherited and previous attributes, the framework must treat value types differently. To do that
it is necessary to declare which types are values types using a `struct` declaration. In other
cases, the framework needs to know the relationships between types to select the most specific type.
A `type` declaration is used to provide that.

| Declaration                                                                       | Meaning                                                                |
| --------------------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| `struct` `` ` ``*Name*`` `; ``                                                    | Declares that the symbol is a value type                               |
| `type` `` ` ``*Name*`` ` `` `<:` `` ` ``*Node*`` ` `` (`,` `` ` ``*Node*`` ` ``)* | Declared a reference type (i.e. class or interface) and its supertypes |

### Synthesized Attributes

| Declaration                                                                             | Meaning                                                                      |
| --------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| `↑` `child`? (`eager`\|`lazy`\|`computed`)? *Node*`.`*Attribute*`:` *Type*`;`           | Synthesized attribute, defaults to lazy                                      |
| `↑` `child`? (`eager`\|`computed`)? *Node*`.`*Attribute*`:` *Type* `=>` *Expression*`;` | Synthesized attribute with default inline expression, defaults to computed   |
| `=` (`eager`\|`lazy`\|`computed`)? *Node*.*Attribute*`;`                                | Synthesized equation, defaults to attribute strategy                         |
| `=` (`eager`\|`computed`)? *Node*.*Attribute* `=>` *Expression*`;`                      | Synthesized equation with inline expression, defaults to computed            |
| `↑` *Node*.*Attribute*`():` *Type*`;`                                                   | Synthesized method attribute, always computed                                |
| `↑` *Node*.*Attribute*`():` *Type* `=>` *Expression*`;`                                 | Synthesized method attribute with default inline expression, always computed |
| `=` *Node*.*Method*`();`                                                                | Synthesized method equation, always computed                                 |
| `=` *Node*.*Method*`()` `=>` *Expression*`;`                                            | Synthesized method equation with inline expression, always computed          |

### Inherited Attributes

| Declaration                                                        | Meaning                                                         |
| ------------------------------------------------------------------ | --------------------------------------------------------------- |
| `↓` (`lazy`\|`computed`)? `child`? *Node*.*Attribute*`:` *Type*`;` | Inherited attribute, defaults to lazy                           |
| `=` *Node*`.`*Selector*`.`*Attribute*`;`                           | Inherited equation                                              |
| `=` *Node*`.`*Selector*`.`*Attribute* `=>` *Expression*`;`         | Inherited equation with inline expression                       |
| `↓` *Node*.*Attribute*`():` *Type*`;`                              | Inherited attribute method, always computed                     |
| `=` *Node*`.`*Selector*`.`*Attribute*`();`                         | Inherited method equation                                       |
| `=` *Node*`.`*Selector*`.`*Attribute*`()` `=>` *Expression*`;`     | Inherited method equation with inline expression                |
| `↓` `*.`*Attribute* `<:` *Type*`;`<sup>1</sup>                     | An inherited attribute family that specifies a common supertype |

1. Only necessary when code generation is not able to determine the type and gives an error.

| Selector                       | Meaning                                                                    |
| ------------------------------ | -------------------------------------------------------------------------- |
| *Child*                        | The named child                                                            |
| `*`                            | All immediate children                                                     |
| *ChildList*`[`*Number*`]`      | The child in the child list at the given index                             |
| *ChildList*`[`*Variable*`]`    | A child in the child list, variable passed to equation                     |
| *ChildCollection*`[*]`         | All children in the child collection                                       |
| *Child*`.**`                   | The named child and its descendants                                        |
| `*.**`                         | All descendants                                                            |
| *ChildList*`[`*Number*`].**`   | The child in the child list at the given index and its descendants         |
| *ChildList*`[`*Variable*`].**` | A child in the child list and its descendants, variable passed to equation |
| *ChildCollection*`[*].**`      | All children in the child collection and their descendants                 |

### Previous Attributes

Previous attributes are similar to inherited attributes except that instead of searching up the tree
for a node to provide the attribute value, nodes are searched in the reverse of a pre-order
traversal. That is, for inherited attributes, values flow down the tree. For previous attributes,
values flow down from the parent to the first child and then across to the next child.

| Declaration                                                | Meaning                                                       |
| ---------------------------------------------------------- | ------------------------------------------------------------- |
| `⮡` `*.`*Attribute*`:` *Type*`;`                           | Previous attribute family                                     |
| `⮡` (`lazy`\|`computed`)? `child`? *Node*`.`*Attribute*`;` | Previous attribute, defaults to lazy                          |
| `=` *Node*`.⮡.`*Attribute*`;`                              | Previous equation                                             |
| `=` *Node*`.⮡.`*Attribute* `=>` *Expression*`;`            | Previous equation with inline expression                      |
| `⮡` *Node*`.`*Attribute*`();`                              | Previous attribute method, always computed                    |
| `=` *Node*`.⮡.`*Attribute*`();`                            | Previous method equation                                      |
| `=` *Node*`.⮡.`*Attribute*`()` `=>` *Expression*`;`        | Previous method equation with inline expression               |

### Circular Attributes

`⟳` *Node*`.`*Attribute*`:` *Type*`;`

### Aggregate Attributes

Aggregate attributes are similar to collection attributes but are simpler in some ways. An aggregate
attribute aggregates values from the subtree rooted at a node (including from that node). If the
same aggregate attribute exists higher in the tree, it will aggregate all values from the lower
aggregate attribute. Thus, the two aggregate attributes do not need to evaluate child attributes
twice.

For aggregate attributes, the aggregation is performed by an aggregating type which is described
with a `from` clause. The `=>` provides an expression to construct a new/empty one. Otherwise the
default constructor will be called. The `with` provides a method to call to add other aggregates to
the aggregate. If no method is specified, `Add` will be used.

Aggregate attributes are evaluated in two phases. In the first phase, nodes that could contribute a
value to the aggregate are collected. This phase does not allow cycles in the evaluation. In the
second phase, values are collected from the nodes to contribute to the aggregate value. This phase,
like regular synthetic and inherited attributes. Can be part of a circular attribute, but do not
directly support cycles themselves.

| Declaration                                                                                              | Meaning                                               |
| -------------------------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| `↗↖` `*.`*Attribute*`:` *Type* `from` *Type* (`=>` *Expression*)? (`with` *Method*)? `done` *Method* `;` | An aggregate attribute family                         |
| `↗↖` *Node*`.`*Attribute*`;`                                                                             | An aggregate attribute on a specific node             |
| `=` *Node*`.↑.`*Attribute*`;`                                                                            | Equation contributing to a parent aggregate attribute |

### Collection Attributes

JastAdd style collection attributes.

| Declaration                                                                                  | Meaning                                         |
| -------------------------------------------------------------------------------------------- | ----------------------------------------------- |
| `→*←` *Node*`.`*Attribute*`:` *Type* (`root` *Node*)? `from` *Type* (`=>` *Expression*)? `;` | A collection attribute                          |
| `=` *Node*`.→*.`*Node*`.`*Attribute* (`when` *cond*)? (`for` *target-exp*)?`;`               | Equation contributing to a collection attribute |

### Rewrites

Rewrite rule names are optional. They exist to distinguish rewrites from each other.

| Declaration           | Meaning                                                  |
| --------------------- | -------------------------------------------------------- |
| `✎` *Node* *Name*?`;` | Add a rewrite rule to the given node with the given name |
