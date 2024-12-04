using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// An ordinary type constructor is one that is declared in source code (as opposed to
/// <see cref="SimpleTypeConstructor"/>s). That is, it was declared with a <c>class</c>,
/// <c>struct</c>, or <c>trait</c> declaration.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class OrdinaryTypeConstructor : TypeConstructor
{
    public TypeConstructorContext Context { get; }
    /// <summary>
    /// Whether the declaration for this type constructor is abstract.
    /// </summary>
    /// <remarks>Classes can be declared abstract with the <c>abstract</c> keyword. Traits are
    /// always abstract. Structs are never abstract.</remarks>
    public bool IsAbstract { get; }
    /// <summary>
    /// Whether types constructed with this type constructor can be instantiated directly. Even if
    /// a type cannot be instantiated, it may be the case that a subtype can. So it is still possible
    /// that instances compatible with the type exist.
    /// </summary>
    public bool CanBeInstantiated => !IsAbstract;

    public StandardName Name { get; }
    TypeName TypeConstructor.Name => Name;

    /// <summary>
    /// The parameters to this type constructor. Commonly referred to as "generic parameters".
    /// </summary>
    public IFixedList<TypeConstructorParameter> Parameters { get; }
    public bool AllowsVariance { get; }
    /// <summary>
    /// Within the type constructor declaration, any generic parameters will appear as type
    /// variables. These are the types of those variables.
    /// </summary>
    public IFixedList<GenericParameterPlainType> GenericParameterPlainTypes { get; }
    public IFixedSet<ConstructedPlainType> Supertypes { get; }
    public TypeSemantics Semantics { get; }

    public OrdinaryTypeConstructor(
        TypeConstructorContext context,
        bool isAbstract,
        StandardName name,
        IEnumerable<TypeConstructorParameter> genericParameters,
        IFixedSet<ConstructedPlainType> supertypes,
        TypeSemantics semantics)
    {
        Context = context;
        Name = name;
        Parameters = genericParameters.ToFixedList();
        Requires.That(Name.GenericParameterCount == Parameters.Count, nameof(genericParameters),
            "Count must match name count");
        AllowsVariance = Parameters.Any(p => p.Variance != TypeVariance.Invariant);
        Semantics = semantics;
        IsAbstract = isAbstract;
        Requires.That(supertypes.Contains(IPlainType.Any), nameof(supertypes),
            "All ordinary type constructors must have `Any` as a supertype.");
        Supertypes = supertypes;
        GenericParameterPlainTypes = Parameters.Select(p => new GenericParameterPlainType(this, p))
                                                     .ToFixedList();
    }

    public ConstructedPlainType Construct(IEnumerable<IPlainType> typeArguments)
    {
        var args = typeArguments.ToFixedList();
        if (args.Count != Parameters.Count)
            throw new ArgumentException("Incorrect number of type arguments.");
        return new(this, args);
    }
    IPlainType TypeConstructor.Construct(IEnumerable<IPlainType> typeArguments)
        => Construct(typeArguments);

    public ConstructedPlainType ConstructWithGenericParameterPlainTypes()
        => Construct(GenericParameterPlainTypes);
    IPlainType TypeConstructor.ConstructWithGenericParameterPlainTypes()
        => ConstructWithGenericParameterPlainTypes();

    public ConstructedPlainType? TryConstructNullary()
        => Parameters.IsEmpty ? new ConstructedPlainType(this, []) : null;
    IPlainType? TypeConstructor.TryConstructNullary() => TryConstructNullary();

    #region Equality
    public bool Equals(TypeConstructor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryTypeConstructor that
               && Context.Equals(that.Context)
               && Name.Equals(that.Name)
               && Parameters.Equals(that.Parameters);
        // GenericParameterPlainTypes is derived from GenericParameters and doesn't need to be compared
    }

    public override int GetHashCode()
        => HashCode.Combine(Context, Name, Parameters);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        Context.AppendContextPrefix(builder);
        builder.Append(Name.ToBareString());
        if (Parameters.IsEmpty) return;

        builder.Append('[');
        builder.AppendJoin(", ", Parameters);
        builder.Append(']');
    }
}
