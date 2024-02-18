using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// A value type without a reference capability.
/// </summary>
public abstract class BareValueType : BareType
{
    public abstract override DeclaredValueType DeclaredType { get; }

    private protected BareValueType(DeclaredType declaredType, FixedList<DataType> typeArguments)
        : base(declaredType, typeArguments) { }

    public abstract override BareValueType AccessedVia(ReferenceCapability capability);

    public abstract override BareValueType With(FixedList<DataType> typeArguments);

    public abstract override ValueType With(ReferenceCapability capability);
}

public sealed class BareValueType<TDeclared> : BareValueType
    where TDeclared : DeclaredValueType
{
    public override TDeclared DeclaredType { get; }

    internal BareValueType(TDeclared declaredType, FixedList<DataType> typeArguments)
        : base(declaredType, typeArguments)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredValueType)}.", nameof(TDeclared));
        DeclaredType = declaredType;
    }

    public override BareValueType<TDeclared> AccessedVia(ReferenceCapability capability)
        => throw new System.NotImplementedException();

    public override BareValueType<TDeclared> With(FixedList<DataType> typeArguments)
        => throw new System.NotImplementedException();

    public override ValueType/*<TDeclared>*/ With(ReferenceCapability capability)
        => throw new System.NotImplementedException();
}
