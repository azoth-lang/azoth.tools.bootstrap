using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// An ordinary type constructor is one that is declared in source code (as opposed to
/// <see cref="SimpleTypeConstructor"/>s). That is, it was declared with a <c>class</c>,
/// <c>struct</c>,  or <c>trait</c> declaration.
/// </summary>
public sealed class OrdinaryTypeConstructor : ITypeConstructor
{
    public IdentifierName ContainingPackage { get; }
    public NamespaceName ContainingNamespace { get; }
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
    TypeName ITypeConstructor.Name => Name;

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
    public IFixedSet<NamedPlainType> Supertypes { get; }
    public TypeSemantics Semantics { get; }

    public OrdinaryTypeConstructor(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        StandardName name,
        IEnumerable<TypeConstructorParameter> genericParameters,
        IFixedSet<NamedPlainType> supertypes,
        TypeSemantics semantics)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        Parameters = genericParameters.ToFixedList();
        Requires.That(Name.GenericParameterCount == Parameters.Count, nameof(genericParameters),
            "Count must match name count");
        AllowsVariance = Parameters.Any(p => p.Variance != TypeVariance.Invariant);
        Semantics = semantics;
        IsAbstract = isAbstract;
        Requires.That(supertypes.Contains(IAntetype.Any), nameof(supertypes),
            "All ordinary type constructors must have `Any` as a supertype.");
        Supertypes = supertypes;
        GenericParameterPlainTypes = Parameters.Select(p => new GenericParameterPlainType(this, p))
                                                     .ToFixedList();
    }

    public NamedPlainType Construct(IEnumerable<IAntetype> typeArguments)
    {
        var args = typeArguments.ToFixedList();
        if (args.Count != Parameters.Count)
            throw new ArgumentException("Incorrect number of type arguments.");
        return new OrdinaryNamedPlainType(this, args);
    }
    IAntetype ITypeConstructor.Construct(IEnumerable<IAntetype> typeArguments)
        => Construct(typeArguments);

    public NamedPlainType ConstructWithGenericParameterPlayTypes()
        => Construct(GenericParameterPlainTypes);
    IAntetype ITypeConstructor.ConstructWithGenericParameterPlainTypes()
        => ConstructWithGenericParameterPlayTypes();

    public IAntetype? TryConstructNullary()
        => Parameters.IsEmpty ? new OrdinaryNamedPlainType(this, []) : null;

    #region Equality
    public bool Equals(ITypeConstructor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryTypeConstructor that
               && ContainingPackage.Equals(that.ContainingPackage)
               && ContainingNamespace.Equals(that.ContainingNamespace)
               && Name.Equals(that.Name)
               && Parameters.Equals(that.Parameters);
        // GenericParameterPlainTypes is derived from GenericParameters and doesn't need to be compared
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, Name, Parameters);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        builder.Append(ContainingPackage);
        builder.Append("::.");
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name.ToBareString());
        if (Parameters.IsEmpty) return;

        builder.Append('[');
        builder.AppendJoin(", ", Parameters);
        builder.Append(']');
    }
}
