using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// An ordinary type constructor is one that is declared in source code (as opposed to
/// <see cref="SimpleTypeConstructor"/>s). That is, it was declared with a <c>class</c>,
/// <c>struct</c>, or <c>trait</c> declaration.
/// </summary>
public sealed class OrdinaryTypeConstructor : BareTypeConstructor
{
    public override BareTypeConstructorContext Context { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether the declaration for this type constructor is abstract.
    /// </summary>
    /// <remarks>Classes can be declared abstract with the <c>abstract</c> keyword. Traits are
    /// always abstract. Structs are never abstract.</remarks>
    public bool IsAbstract { [DebuggerStepThrough] get; }

    public override bool IsDeclaredConst { [DebuggerStepThrough] get; }

    /// <summary>
    /// What kind of type this is (e.g. class, trait, or struct).
    /// </summary>
    public TypeKind Kind { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether types constructed with this type constructor can be instantiated directly. Even if
    /// a type cannot be instantiated, it may be the case that a subtype can. So it is still possible
    /// that instances compatible with the type exist.
    /// </summary>
    public override bool CanBeInstantiated => !IsAbstract;

    /// <summary>
    /// Whether this type can have fields.
    /// </summary>
    /// <remarks>Even if a type cannot have fields, a subtype still could.</remarks>
    public override bool CanHaveFields => Kind != TypeKind.Trait;

    public override bool CanBeBaseType => Kind == TypeKind.Class;
    public override bool CanBeSupertype => Kind != TypeKind.Struct;

    public override OrdinaryName Name { [DebuggerStepThrough] get; }

    /// <summary>
    /// The parameters of this type constructor. Commonly referred to as "generic parameters".
    /// </summary>
    public override IFixedList<TypeConstructorParameter> Parameters { [DebuggerStepThrough] get; }
    public override bool AllowsVariance { [DebuggerStepThrough] get; }
    public override bool HasIndependentParameters { [DebuggerStepThrough] get; }

    public override IFixedList<GenericParameterTypeConstructor> ParameterTypeFactories { [DebuggerStepThrough] get; }

    public override BareType? BaseType { [DebuggerStepThrough] get; }
    public override IFixedSet<BareType> Supertypes { [DebuggerStepThrough] get; }
    public override TypeSemantics? Semantics
        => Kind == TypeKind.Struct ? TypeSemantics.Value : TypeSemantics.Reference;

    public OrdinaryTypeConstructor(
        BareTypeConstructorContext context,
        bool isAbstract,
        bool isDeclaredConst,
        TypeKind kind,
        OrdinaryName name,
        IEnumerable<TypeConstructorParameter> genericParameters,
        BareType? baseType,
        IEnumerable<BareType> supertypes)
    {
        Requires.That((kind == TypeKind.Trait).Implies(isAbstract), nameof(isAbstract), "Traits must be abstract.");
        Requires.That((kind == TypeKind.Struct).Implies(!isAbstract), nameof(isAbstract), "Structs cannot be abstract.");
        Context = context;
        IsAbstract = isAbstract;
        Kind = kind;
        Name = name;
        Parameters = genericParameters.ToFixedList();
        Requires.That(Name.GenericParameterCount == Parameters.Count, nameof(genericParameters),
            "Count must match name count");
        AllowsVariance = Parameters.Any(p => p.Variance != TypeParameterVariance.Invariant);
        HasIndependentParameters = Parameters.Any(p => p.HasIndependence);

        Requires.That((baseType is not null).Implies(kind == TypeKind.Class), nameof(baseType),
            "Only classes can have base types.");
        Requires.That((baseType is not null).Implies(baseType!.TypeConstructor.CanBeBaseType),
            nameof(baseType), "Must be a class.");
        BaseType = baseType;
        Supertypes = supertypes.AppendValue(baseType).ToFixedSet();
        Requires.That(Supertypes.Contains(BareType.Any), nameof(supertypes),
            "All ordinary type constructors must have `Any` as a supertype.");
        IsDeclaredConst = isDeclaredConst;
        ParameterTypeFactories = Parameters.Select(p => new GenericParameterTypeConstructor(this, p)).ToFixedList();
    }

    /// <summary>
    /// Make a version of this type for use as the default constructor or initializer parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public CapabilityType ToDefaultConstructorSelf()
        => SelfTypeConstructor.ConstructWithParameterTypes().With(Capability.InitMutable);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor or initializer.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public CapabilityType ToDefaultConstructorReturn()
        => SelfTypeConstructor.ConstructWithParameterTypes().With(IsDeclaredConst ? Capability.Constant : Capability.Isolated);

    /// <summary>
    /// Determine the return type of a constructor or initializer with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public CapabilityType ToConstructorReturn(CapabilityType selfParameterType, IEnumerable<ParameterType> parameterTypes)
    {
        var bareType = ConstructWithParameterTypes();
        if (IsDeclaredConst) return bareType.With(Capability.Constant);
        // Read only self constructors cannot return `mut` or `iso`
        if (!selfParameterType.Capability.AllowsWrite)
            return bareType.With(Capability.Read);

        var capability = parameterTypes.Any(BreaksIsolation) ? Capability.Mutable : Capability.Isolated;
        return bareType.With(capability);
    }

    private static bool BreaksIsolation(ParameterType parameterType)
        => BreaksIsolation(parameterType.IsLent, parameterType.Type);
    private static bool BreaksIsolation(bool isLent, Type type)
        => type switch
        {
            CapabilityType when isLent => false,
            CapabilityType { Capability: var c } when c == Capability.Constant || c == Capability.Isolated => false,
            OptionalType { Referent: var referent } when !BreaksIsolation(isLent, referent) => false,
            NeverType => false,
            _ => true
        };

    public override BarePlainType Construct(
        BarePlainType? containingType,
        IFixedList<PlainType> arguments)
    {
        if (arguments.Count != Parameters.Count)
            throw new ArgumentException("Incorrect number of type arguments.");
        return new(this, containingType, arguments);
    }

    public override PlainType? TryConstructNullaryPlainType(BarePlainType? containingType)
        => Parameters.IsEmpty ? new BarePlainType(this, containingType, []) : null;

    #region Equality
    public override bool Equals(BareTypeConstructor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryTypeConstructor that
               && Context.Equals(that.Context)
               && Name.Equals(that.Name)
               && Parameters.Equals(that.Parameters);
        // ParameterPlainTypes is derived from Parameters and doesn't need to be compared
    }

    public override int GetHashCode()
        => HashCode.Combine(Context, Name, Parameters);
    #endregion

    public override void ToString(StringBuilder builder)
    {
        Context.AppendContextPrefix(builder, containingType: null);
        builder.Append(Name.ToBareString());
        if (Parameters.IsEmpty) return;

        builder.Append('[');
        builder.AppendJoin(", ", Parameters);
        builder.Append(']');
    }
}
