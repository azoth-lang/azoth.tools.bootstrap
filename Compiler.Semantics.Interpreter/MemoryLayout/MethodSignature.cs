using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class MethodSignature : IEquatable<MethodSignature>
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public NonVoidType SelfType { [DebuggerStepThrough] get; }
    public IFixedList<ParameterType> ParameterTypes { [DebuggerStepThrough] get; }
    public Type ReturnType { [DebuggerStepThrough] get; }
    private readonly int hashCode;

    public MethodSignature(
        IdentifierName name,
        NonVoidType selfType,
        IFixedList<ParameterType> parameterTypes,
        Type returnType)
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
        var selfType = SelfType;
        return Name.Equals(other.Name)
               && SelfType.CanOverride(selfType.TypeReplacements.ApplyTo(other.SelfType))
               && ParametersCompatible(selfType, other)
               && ReturnType.ReturnCanOverride(selfType.TypeReplacements.ApplyTo(other.ReturnType));
    }

    private bool ParametersCompatible(NonVoidType selfType, MethodSignature other)
    {
        if (ParameterTypes.Count != other.ParameterTypes.Count) return false;
        foreach (var (paramType, baseParamType) in ParameterTypes.EquiZip(other.ParameterTypes.Select(selfType.TypeReplacements.ApplyTo)))
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
