using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// The sharing relationships implied between parameters.
/// </summary>
public sealed class ParameterSharingRelation
{
    public FixedList<BindingSymbol> Symbols { get; }
    public FixedSet<SharingSetSnapshot> SharingSets { get; }

    public ParameterSharingRelation(IEnumerable<BindingSymbol> parameterSymbols)
    {
        Symbols = parameterSymbols.ToFixedList();
        SharingSets = BuildSharingSets(Symbols);
    }

    #region Static Sharing Sets Builder Methods
    private static FixedSet<SharingSetSnapshot> BuildSharingSets(FixedList<BindingSymbol> parameterSymbols)
    {
        var sharingSets = new HashSet<SharingSet>();
        uint lentParameterNumber = 0;
        SharingSet? nonLentParametersSet = null;
        foreach (var parameterSymbol in parameterSymbols)
        {
            var setForParameter = DeclareVariable(sharingSets, parameterSymbol);
            if (setForParameter is null) continue;
            if (parameterSymbol.DataType is not ReferenceType { Capability: var capability }) continue;

            // These capabilities don't have to worry about external references
            if (capability == ReferenceCapability.Isolated || capability == ReferenceCapability.Constant
                                                           || capability == ReferenceCapability.Identity)
                continue;

            // Create external references so that they can't be frozen or moved
            if (parameterSymbol.IsLentBinding)
                DeclareLentParameterReference(setForParameter, ++lentParameterNumber);
            else
            {
                nonLentParametersSet ??= DeclareVariable(sharingSets, ExternalReference.NonLentParameters, false);

                if (!nonLentParametersSet.UnionWith(setForParameter))
                    throw new UnreachableCodeException("Union failed.");

                sharingSets.Remove(setForParameter);
            }
        }

        return sharingSets.Select(s => s.Snapshot()).ToFixedSet();
    }

    private static SharingSet? DeclareVariable(HashSet<SharingSet> sets, BindingSymbol symbol)
    {
        // Other types don't participate in sharing
        if (!symbol.SharingIsTracked()) return null;

        BindingVariable variable = symbol;
        return DeclareVariable(sets, variable, variable.IsLent);
    }

    private static SharingSet DeclareVariable(HashSet<SharingSet> sets, ISharingVariable variable, bool isLent)
    {
        var set = new SharingSet(variable, isLent);
        sets.Add(set);
        return set;
    }

    public static void DeclareLentParameterReference(SharingSet set, uint lentParameterNumber)
        => set.Declare(ExternalReference.CreateLentParameter(lentParameterNumber));
    #endregion
}
