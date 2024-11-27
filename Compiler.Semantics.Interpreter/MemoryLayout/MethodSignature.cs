using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal class MethodSignature : IEquatable<MethodSignature>
{
    public IdentifierName Name { get; }
    public SelfParameterType SelfType { get; }
    public IFixedList<ParameterType> ParameterTypes { get; }
    public IType ReturnType { get; }
    private readonly int hashCode;

    public MethodSignature(
        IdentifierName name,
        SelfParameterType selfType,
        IFixedList<ParameterType> parameterTypes,
        IType returnType)
    {
        Name = name;
        SelfType = selfType;
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
        hashCode = HashCode.Combine(Name, SelfType, ParameterTypes, ReturnType);
    }

    public bool EqualsOrOverrides(MethodSignature other)
    {
        if (ReferenceEquals(this, other)) return true;
        var selfType = (NonEmptyType)SelfType.Type;
        return Name.Equals(other.Name)
               && SelfType.CanOverride(selfType.ReplaceTypeParametersIn(other.SelfType))
               && ParametersCompatible(selfType, other)
               && ReturnType.ReturnCanOverride(selfType.ReplaceTypeParametersIn(other.ReturnType));
    }

    private bool ParametersCompatible(NonEmptyType selfType, MethodSignature other)
    {
        if (ParameterTypes.Count != other.ParameterTypes.Count) return false;
        foreach (var (paramType, baseParamType) in ParameterTypes.EquiZip(other.ParameterTypes.Select(selfType.ReplaceTypeParametersIn)))
            // A null baseParamType means that the parameter was replaced to `void` and dropped out
            // of the parameter list.
            if (baseParamType is null || !paramType.CanOverride(baseParamType))
                return false;

        return true;
    }

    #region Equality
    public bool Equals(MethodSignature? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name.Equals(other.Name)
               && SelfType.Equals(other.SelfType)
               && ParameterTypes.Equals(other.ParameterTypes)
               && ReturnType.Equals(other.ReturnType);
    }

    public override bool Equals(object? obj) => Equals(obj as MethodSignature);

    public override int GetHashCode() => hashCode;

    public static bool operator ==(MethodSignature? left, MethodSignature? right)
        => Equals(left, right);

    public static bool operator !=(MethodSignature? left, MethodSignature? right)
        => !Equals(left, right);
    #endregion

    public override string ToString()
    {
        var parameterSeparator = ParameterTypes.Any() ? ", " : "";
        string parameters = string.Join(", ", ParameterTypes.Select(d => d.ToILString()));
        return $"{Name}({SelfType.ToILString()}{parameterSeparator}{parameters}) -> {ReturnType.ToILString()}";
    }
}
