using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute.
/// </summary>
[Closed(typeof(AspectAttributeModel), typeof(PropertyModel))]
public abstract class AttributeModel
{
    public static IEqualityComparer<AttributeModel> NameAndTypeComparer { get; }
        = EqualityComparer<AttributeModel>.Create((p1, p2) => p1?.Name == p2?.Name && p1?.Type == p2?.Type,
            p => HashCode.Combine(p.Name, p.Type));

    public static IEqualityComparer<AttributeModel> NameComparer { get; }
        = EqualityComparer<AttributeModel>.Create((p1, p2) => p1?.Name == p2?.Name, p => HashCode.Combine(p.Name));

    public abstract AttributeSyntax? Syntax { get; }
    public abstract TreeNodeModel Node { get; }
    public abstract string Name { get; }
    public abstract TypeModel Type { get; }
}
