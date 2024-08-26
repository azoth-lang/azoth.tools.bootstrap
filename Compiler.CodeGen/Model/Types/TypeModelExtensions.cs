using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public static class TypeModelExtensions
{
    public static IEnumerable<T> MostSpecificTypes<T>(this IEnumerable<T> types)
        where T : TypeModel
    {
        var mostSpecific = new List<T>();
        foreach (var type in types.Distinct())
        {
            for (var i = mostSpecific.Count - 1; i >= 0; i--)
            {
                var mostSpecificType = mostSpecific[i];
                if (mostSpecificType.IsSubtypeOf(type)) goto nextType;
                if (type.IsSubtypeOf(mostSpecificType)) mostSpecific.RemoveAt(i);
            }

            mostSpecific.Add(type);

        nextType:;
        }

        return mostSpecific;
    }

    public static IEnumerable<T> MostGeneralTypes<T>(this IEnumerable<T> types)
        where T : TypeModel
    {
        var mostGeneral = new List<T>();
        foreach (var type in types.Distinct())
        {
            for (var i = mostGeneral.Count - 1; i >= 0; i--)
            {
                var mostGeneralType = mostGeneral[i];
                if (type.IsSubtypeOf(mostGeneralType)) goto nextType;
                if (mostGeneralType.IsSubtypeOf(type)) mostGeneral.RemoveAt(i);
            }

            mostGeneral.Add(type);

        nextType:;
        }

        return mostGeneral;
    }
}
