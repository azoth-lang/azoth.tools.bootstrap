using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

internal static class SymbolBuilder
{
    public static ParameterType Param(NonVoidType type)
        => new ParameterType(false, type);

    public static IFixedList<ParameterType> Params(params NonVoidType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params)
        => new(containingSymbol, name, new FunctionType(@params, IType.Void));

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params, IType @return)
        => new(containingSymbol, name, new FunctionType(@params, @return));

    public static MethodSymbol Method(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, IFixedList<ParameterType> @params)
        => new(containingSymbol, MethodKind.Standard, name, selfParam, @params, IType.Void);

    public static MethodSymbol Method(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, IFixedList<ParameterType> @params, IType @return)
        => new(containingSymbol, MethodKind.Standard, name, selfParam, @params, @return);

    public static MethodSymbol Getter(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, IType @return)
        => new(containingSymbol, MethodKind.Getter, name, selfParam, FixedList.Empty<ParameterType>(), @return);

    public static MethodSymbol Setter(TypeSymbol containingSymbol, IdentifierName name, NonVoidType selfParam, ParameterType parameter)
        => new(containingSymbol, MethodKind.Setter, name, selfParam, FixedList.Create(parameter), IType.Void);
}
