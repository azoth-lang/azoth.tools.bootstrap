using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

internal static class ParameterExtensions
{
    public static IEnumerable<Parameter> MergeByName(this IEnumerable<Parameter> parameters)
        // GroupBy guarantees that groups are returned in the order of the original collection
        => parameters.GroupBy(p => p.Name).Select(g => Parameter.Create(BottomType(g), g.Key));

    private static NonVoidType TopType(IEnumerable<Parameter> parameters)
        => TopType(parameters.Select(p => p.Type));

    private static NonVoidType TopType(IEnumerable<NonVoidType> types)
        => types.Aggregate((t1, t2) => t2.IsSubtypeOf(t1) ? t1 : (t1.IsSubtypeOf(t2) ? t2 : throw new InvalidOperationException($"No top type for '{t1}' and '{t2}'.")));

    private static NonVoidType BottomType(IEnumerable<Parameter> parameters)
        => BottomType(parameters.Select(p => p.Type));

    private static NonVoidType BottomType(IEnumerable<NonVoidType> types)
        => types.Aggregate((t1, t2)
            => t1.IsSubtypeOf(t2)
                ? t1
                : (t2.IsSubtypeOf(t1)
                    ? t2
                    : throw new InvalidOperationException($"No bottom type for '{t1}' and '{t2}'.")));
}
