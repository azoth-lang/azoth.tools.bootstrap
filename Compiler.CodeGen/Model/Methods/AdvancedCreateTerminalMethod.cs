using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed record AdvancedCreateTerminalMethod : AdvancedCreateMethod
{
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }
    public override IFixedList<Parameter> AllParameters { get; }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AdvancedCreateTerminalMethod(Pass pass, Rule toRule) : base(pass, toRule)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Requires.That(nameof(toRule), toRule.IsTerminal, "Must be a terminal");
        AdditionalParameters = toRule.ModifiedProperties.Select(p => p.Parameter).ToFixedList();
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();
    }

    public override IEnumerable<Method> GetMethodsCalled()
    {
        yield return Pass.SimpleCreateMethods.Single(m => m.ToRule == ToRule);

        var fromCoreType = FromType.ToNonOptional();
        switch (fromCoreType)
        {
            default:
                throw ExhaustiveMatch.Failed(fromCoreType);
            case CollectionType collectionType:
                yield return Pass.TransformMethods.Single(t => t.FromType == collectionType.ElementType);
                break;
            case SymbolType _:
                foreach (var property in ToRule.DifferentChildProperties)
                {
                    var fromType = property.ComputeFromType().ToNonOptional();
                    yield return Pass.TransformMethods.Single(t => t.FromType == fromType);
                }
                break;
        }
    }
}
