using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

/// <summary>
/// A simple create method is one that created a new node from a previous node and the value of any
/// changed properties including child nodes.
/// </summary>
public sealed record SimpleCreateMethod : Method
{
    public Rule Rule { get; }
    public Parameter From { get; }
    public Symbol To => Rule.Defines;
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }
    public IFixedList<Parameter> AllParameters { get; }
    public Type ReturnType { get; }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SimpleCreateMethod(Rule rule)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(rule.ExtendsRule!.DefinesType)
    {
        Rule = rule;
        From = Parameter.Create(rule.ExtendsRule!.DefinesType, Parameter.FromName);
        AdditionalParameters = rule.DifferentProperties.Select(p => p.Parameter).ToFixedList();
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();
        ReturnType = rule.DefinesType;
    }

    public override IEnumerable<Method> GetMethodsCalled() => Enumerable.Empty<Method>();
}
