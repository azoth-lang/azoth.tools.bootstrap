using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public static IMaybeType AccessedVia(this IMaybeType type, IMaybeType contextType)
        => contextType switch
        {
            CapabilityType t => type.AccessedVia(t.Capability),
            CapabilitySetSelfType t => type.AccessedVia(t.CapabilitySet),
            SelfViewpointType t => type.AccessedVia(t.Referent).AccessedVia(t.CapabilitySet),
            _ => type
        };

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public static IMaybeType AccessedVia(this IMaybeType type, ICapabilityConstraint capability)
        => type switch
        {
            Type t => t.AccessedVia(capability),
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public static Type AccessedVia(this Type type, ICapabilityConstraint capability)
        => type switch
        {
            VoidType t => t,
            NonVoidType t => t.AccessedVia(capability),
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public static NonVoidType AccessedVia(this NonVoidType type, ICapabilityConstraint capability)
        => type switch
        {
            CapabilityType t => t.AccessedVia(capability),
            GenericParameterType t => t.AccessedVia(capability),
            FunctionType t => t,
            NeverType t => t,
            RefType t => t.AccessedVia(capability),
            // TODO shouldn't this combine with the capability set?
            CapabilitySetSelfType t => t,
            // TODO shouldn't this combine with the capability set?
            //CapabilityViewpointType t => t.Referent.AccessedVia(t.Capability.AccessedVia(capability)),
            CapabilityViewpointType t => t,
            // TODO shouldn't this combine with the capability set?
            CapabilitySetRestrictedType t => t,
            // TODO shouldn't this combine with the capability set?
            SelfViewpointType t => t,
            // TODO should it modify the referent
            OptionalType t => t,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public static NonVoidType AccessedVia(this CapabilityType self, ICapabilityConstraint capability)
    {
        var newCapability = self.Capability.AccessedVia(capability);
        switch (newCapability)
        {
            case Capability c:
                // Access can affect type arguments
                var newArguments = self.ArgumentsAccessedVia(capability);
                if (ReferenceEquals(newArguments, self.Arguments)
                    && ReferenceEquals(c, self.Capability)) return self;
                return self.BareType.WithReplacement(newArguments).WithModified(c);
            case CapabilitySet c:
                return new SelfViewpointType(c, self);
            default:
                throw ExhaustiveMatch.Failed(capability);
        }
    }

    private static IFixedList<Type> ArgumentsAccessedVia(this CapabilityType self, ICapabilityConstraint capability)
    {
        if (!self.HasIndependentTypeArguments) return self.Arguments;
        var newTypeArguments = new List<Type>(self.Arguments.Count);
        var typesReplaced = false;
        foreach (var arg in self.Arguments)
        {
            var newTypeArg = arg.AccessedVia(capability);
            typesReplaced |= !ReferenceEquals(newTypeArg, arg);
            newTypeArguments.Add(newTypeArg);
        }

        return typesReplaced ? newTypeArguments.ToFixedList() : self.Arguments;
    }

    public static NonVoidType AccessedVia(this GenericParameterType self, ICapabilityConstraint capability)
    {
        // Independent type parameters are not affected by the capability
        if (self.Parameter.HasIndependence) return self;
        return capability switch
        {
            Capability c => CapabilityViewpointType.Create(c, self),
            CapabilitySet c => new SelfViewpointType(c, self),
            _ => throw ExhaustiveMatch.Failed(capability),
        };
    }

    public static NonVoidType AccessedVia(this RefType self, ICapabilityConstraint capability)
    {
        switch (capability)
        {
            case Capability c:
                var newReferent = self.Referent.AccessedVia(capability);
                var newIsMutableBinding = self.IsMutableBinding && c.AllowsWrite;
                if (ReferenceEquals(newReferent, self.Referent)
                    && newIsMutableBinding == self.IsMutableBinding)
                    return self;
                return RefType.CreateWithoutPlainType(self.IsInternal, newIsMutableBinding, newReferent);
            case CapabilitySet c:
                return new SelfViewpointType(c, self);
            default:
                throw ExhaustiveMatch.Failed(capability);
        }
    }
}
