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
        var sharing = new SharingRelation();
        bool nonLentParametersReferenceDeclared = false;
        uint lentParameterNumber = 0;
        foreach (var parameterSymbol in Symbols)
        {
            sharing.Declare(parameterSymbol);
            if (parameterSymbol.DataType is not ReferenceType { Capability: var capability }) continue;

            // These capabilities don't have to worry about external references
            if (capability == ReferenceCapability.Isolated || capability == ReferenceCapability.Constant
                                                           || capability == ReferenceCapability.Identity)
                continue;

            // Create external references so that they can't be frozen or moved
            if (parameterSymbol.IsLentBinding)
                sharing.DeclareLentParameterReference(parameterSymbol, ++lentParameterNumber);
            else
            {
                if (!nonLentParametersReferenceDeclared)
                {
                    sharing.DeclareNonLentParametersReference();
                    nonLentParametersReferenceDeclared = true;
                }

                sharing.Union(ExternalReference.NonLentParameters, parameterSymbol, null);
            }
        }

        SharingSets = sharing.SharingSets.Select(s => s.Snapshot()).ToFixedSet();
    }
}
