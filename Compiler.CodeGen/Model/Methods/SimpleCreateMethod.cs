using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

/// <summary>
/// A simple create method is one that created a new node from a previous node and the value of any
/// changed properties including child nodes.
/// </summary>
public sealed record SimpleCreateMethod : CreateMethod
{
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SimpleCreateMethod(Pass pass, Rule toRule)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(pass, toRule)
    {
        AdditionalParameters = toRule.DifferentProperties.Select(p => p.Parameter).ToFixedList();
    }

    public override IEnumerable<Method> GetMethodsCalled() => Enumerable.Empty<Method>();
}
