using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

internal static class ParameterExtensions
{
    public static IEnumerable<Parameter> MergeByName(this IEnumerable<Parameter> parameters)
        // GroupBy guarantees that groups are returned in the order of the original collection
        => parameters.GroupBy(p => p.Name).Select(g => Parameter.Create(BottomType(g), g.Key));

    public static bool CanMergeByName(this IEnumerable<Parameter> parameters)
        => parameters.GroupBy(p => p.Name).All(HasBottomType);

    private static NonVoidType BottomType(IEnumerable<Parameter> parameters)
        => BottomType(parameters.Select(p => p.Type));

    private static NonVoidType BottomType(IEnumerable<NonVoidType> types)
        => types.Aggregate(BottomType);

    private static NonVoidType BottomType(NonVoidType t1, NonVoidType t2)
    {
        if (TryBottomType(t1, t2, out var bottomType))
            return bottomType;
        throw new InvalidOperationException($"No bottom type for '{t1}' and '{t2}'.");
    }

    private static bool TryBottomType(NonVoidType t1, NonVoidType t2, [NotNullWhen(true)] out NonVoidType? bottomType)
    {
        if (t1.IsSubtypeOf(t2))
            bottomType = t1;
        else if (t2.IsSubtypeOf(t1))
            bottomType = t2;
        else
        {
            bottomType = null;
            return false;
        }

        return true;
    }

    private static bool HasBottomType(IEnumerable<Parameter> parameters)
        => parameters.Select(p => p.Type).HasBottomType();

    private static bool HasBottomType(this IEnumerable<NonVoidType> types)
    {
        NonVoidType? bottomType = null;
        foreach (var type in types)
        {
            if (bottomType is null)
            {
                bottomType = type;
                continue;
            }
            if (TryBottomType(bottomType, type, out var newBottomType))
                bottomType = newBottomType;
            else
                return false;
        }
        return true;
    }
}
