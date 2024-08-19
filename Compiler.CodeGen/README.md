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

### Synthesized Attributes

| Declaration                                                                             | Meaning                                                                      |
| --------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| `↑` `child`? (`eager`\|`lazy`\|`computed`)? *Node*`.`*Attribute*`:` *Type*`;`           | Synthesized attribute, defaults to lazy                                      |
| `↑` `child`? (`eager`\|`computed`)? *Node*`.`*Attribute*`:` *Type* `=>` *Expression*`;` | Synthesized attribute with default inline expression, defaults to computed   |
| `=` (`eager`\|`lazy`\|`computed`)? *Node*.*Attribute*(`:` *Type*)?`;`                   | Synthesized equation, defaults to attribute strategy                         |
| `=` (`eager`\|`computed`)? *Node*.*Attribute*(`:` *Type*)? `=>` *Expression*`;`         | Synthesized equation with inline expression, defaults to computed            |
| `↑` `child`? *Node*.*Attribute*`(`*Params*`):` *Type*`;`                                | Synthesized method attribute, always computed                                |
| `↑` `child`? *Node*.*Attribute*`(`*Params*`):` *Type* `=>` *Expression*`;`              | Synthesized method attribute with default inline expression, always computed |
| `=` *Node*.*Method*`(`*Params*`)`(`:` *Type*)?`;`                                       | Synthesized method equation, always computed                                 |
| `=` *Node*.*Method*`(`*Params*`)`(`:` *Type*)? `=>` *Expression*`;`                     | Synthesized method equation with inline expression, always computed          |

### Inherited Attributes

| Declaration                                                        | Meaning                                                 |
| ------------------------------------------------------------------ | ------------------------------------------------------- |
| `↓` (`lazy`\|`computed`)? `child`? *Node*.*Attribute*`:` *Type*`;` | Inherited attribute, defaults to lazy                   |
| `=` *Node*`.`*Selector*`.`*Attribute*`;`                           | Inherited equation                                      |
| `=` *Node*`.`*Selector*`.`*Attribute* `=>` *Expression*`;`         | Inherited equation with inline expression               |
| `↓` `child`? *Node*.*Attribute*`():` *Type*`;`                     | Inherited attribute method, always computed             |
| `=` *Node*`.`*Selector*`.`*Attribute*`();`                         | Inherited method equation                               |
| `=` *Node*`.`*Selector*`.`*Attribute*`()` `=>` *Expression*`;`     | Inherited method equation with inline expression        |
| `↓` `*.`*Attribute* `<:` *Type*`;`                                 | Specify a common supertype of the attribute<sup>1</sup> |

1. Only necessary when code generation is not able to determine the type and gives an error.

| Selector                       | Meaning                                                                    |
| ------------------------------ | -------------------------------------------------------------------------- |
| *Child*                        | The named child                                                            |
| `*`                            | All immediate children                                                     |
| *ChildList*`[`*Number*`]`      | The child in the child list at the given index                             |
| *ChildList*`[`*Variable*`]`    | A child in the child list, variable passed to equation                     |
| *ChildList*`[*]`               | All children in the child list                                             |
| *Child*`.**`                   | The named child and its descendants                                        |
| `*.**`                         | All descendants                                                            |
| *ChildList*`[`*Number*`].**`   | The child in the child list at the given index and its descendants         |
| *ChildList*`[`*Variable*`].**` | A child in the child list and its descendants, variable passed to equation |
| *ChildList*`[*].**`            | All children in the child list and their descendants                       |

### Previous Attributes

Previous attributes are similar to inherited attributes except that instead of searching up the tree for a node to provide the attribute value, nodes are searched in the reverse of a pre-order traversal. That is, for inherited attributes, values flow down the tree. For previous attributes, values flow down from the parent to the first child and then across to the next child.

`⮡` `child`? *Node*.*Attribute*`:` *Type*`;`

### Circular Attributes

`⟳` *Node*`.`*Attribute*`:` *Type*`;`

### Collection Attributes

May want to support both JastAdd style collection attributes and aggregating attributes.

`[*]` *Node*`.`*Attribute*`:` *Type* (`root` *Node*)?`;` // TODO how to set builders?

*Node* `contributes` *exp* `when` *cond* *to* *Node*`.`*Attribute* `for` *target-exp*`;`
*Node* `contributes` *exp* `when` *cond* *to* *Node*`.`*Attribute*`;`

### Rewrites
