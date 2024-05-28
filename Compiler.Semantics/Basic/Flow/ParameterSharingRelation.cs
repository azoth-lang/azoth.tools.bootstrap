using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// The sharing relationships implied between parameters.
/// </summary>
public sealed class ParameterSharingRelation
{
    public IFixedList<BindingSymbol> Symbols { get; }
    public IFixedSet<SharingSetSnapshot> SharingSets { get; }

    public ParameterSharingRelation(IEnumerable<BindingSymbol> parameterSymbols)
    {
        Symbols = parameterSymbols.ToFixedList();
        SharingSets = BuildSharingSets(Symbols);
    }

    public override string ToString()
        => string.Join(", ", SharingSets.Select(s => $"{{{string.Join(", ", s)}}}"));

    #region Static Sharing Sets Builder Methods
    private static IFixedSet<SharingSetSnapshot> BuildSharingSets(IFixedList<BindingSymbol> parameterSymbols)
    {
        var sharingSets = new HashSet<SharingSetMutable>();
        uint lentParameterNumber = 0;
        SharingSetMutable? nonLentParametersSet = null;
        foreach (var parameterSymbol in parameterSymbols)
        {
            var setForParameter = DeclareVariable(sharingSets, parameterSymbol);
            if (setForParameter is null) continue;
            if (parameterSymbol.Type.ToUpperBound() is not ReferenceType { Capability: var capability }) continue;

            // These capabilities don't have to worry about external references
            if (capability == Capability.Isolated || capability == Capability.TemporarilyIsolated
                || capability == Capability.Constant || capability == Capability.Identity)
                continue;

            // Create external references so that they can't be frozen or moved
            if (parameterSymbol.IsLentBinding)
                DeclareLentParameterReference(setForParameter, ++lentParameterNumber);
            else
            {
                nonLentParametersSet ??= DeclareVariable(sharingSets, ExternalReference.NonLentParameters, false);

                if (!nonLentParametersSet.UnionWith(setForParameter))
                    throw new UnreachableException("Union failed.");

                sharingSets.Remove(setForParameter);
            }
        }

        return sharingSets.Select(s => s.Snapshot()).ToFixedSet();
    }

    private static SharingSetMutable? DeclareVariable(HashSet<SharingSetMutable> sets, BindingSymbol symbol)
    {
        // Other types don't participate in sharing
        if (!symbol.SharingIsTracked()) return null;

        BindingVariable variable = symbol;
        return DeclareVariable(sets, variable, variable.IsLent);
    }

    private static SharingSetMutable DeclareVariable(HashSet<SharingSetMutable> sets, ISharingVariable variable, bool isLent)
    {
        var set = new SharingSetMutable(variable, isLent);
        sets.Add(set);
        return set;
    }

    public static void DeclareLentParameterReference(SharingSetMutable set, uint lentParameterNumber)
        => set.Declare(ExternalReference.CreateLentParameter(lentParameterNumber));
    #endregion
}
