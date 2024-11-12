using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public static class TypeModelExtensions
{
    /// <summary>
    /// For a set of types, find the most specific types. A type is more specific than another if it
    /// is a subtype. If the collection contains a type that is a subtype of all other types in the
    /// collection that will be returned.
    /// </summary>
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

    /// <summary>
    /// For a set of types, Find the most general types. That is, the types that are supertypes of
    /// all other types in the collection.
    /// </summary>
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
