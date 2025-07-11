using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A class that provides state convenience methods for building various symbols.
/// </summary>
public static class SymbolBuilder
{
    public static ParameterType Param(NonVoidType type)
        => new ParameterType(false, type);

    public static IFixedList<ParameterType> Params(params NonVoidType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params)
        => new(containingSymbol, name, new FunctionType(@params, Type.Void));

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params, Type @return)
        => new(containingSymbol, name, new FunctionType(@params, @return));

    public static InitializerSymbol Initializer(OrdinaryTypeSymbol containingTypeSymbol, CapabilityType selfParameterType, IFixedList<ParameterType> @params)
    => new(containingTypeSymbol, null, selfParameterType, @params);

    public static MethodSymbol Method(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, IFixedList<ParameterType> @params)
        => new(containingSymbol, MethodKind.Standard, name, selfParam, @params, Type.Void);

    public static MethodSymbol Method(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, IFixedList<ParameterType> @params, Type @return)
        => new(containingSymbol, MethodKind.Standard, name, selfParam, @params, @return);

    public static MethodSymbol Getter(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, Type @return)
        => new(containingSymbol, MethodKind.Getter, name, selfParam, FixedList.Empty<ParameterType>(), @return);

    public static MethodSymbol Setter(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, ParameterType parameter)
        => new(containingSymbol, MethodKind.Setter, name, selfParam, FixedList.Create(parameter), Type.Void);

    public static BareType BareSelfType(BareType bareType, params Type[] arguments)
    {
        var selfTypeConstructor = bareType.TypeConstructor.SelfTypeConstructor;
        var bareSelfPlainType = new BarePlainType(selfTypeConstructor, bareType.PlainType, arguments.Select(a => a.PlainType));
        var bareSelfType = new BareType(bareSelfPlainType, bareType, arguments.ToFixedList());
        return bareSelfType;
    }
}
