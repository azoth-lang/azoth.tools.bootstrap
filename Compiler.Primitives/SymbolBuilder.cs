using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

internal static class SymbolBuilder
{
    public static SelfParameterType SelfParam(INonVoidType type) => new(false, type);

    public static ParameterType Param(INonVoidType type)
        => new ParameterType(false, type);

    public static IFixedList<ParameterType> Params(params INonVoidType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params)
        => new(containingSymbol, name, new FunctionType(@params, IType.Void));

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params, IType @return)
        => new(containingSymbol, name, new FunctionType(@params, @return));

    public static MethodSymbol Method(TypeSymbol containingSymbol, IdentifierName name, SelfParameterType selfParam, IFixedList<ParameterType> @params)
        => new(containingSymbol, MethodKind.Standard, name, selfParam, @params, IType.Void);

    public static MethodSymbol Method(TypeSymbol containingSymbol, IdentifierName name, SelfParameterType selfParam, IFixedList<ParameterType> @params, IType @return)
        => new(containingSymbol, MethodKind.Standard, name, selfParam, @params, @return);

    public static MethodSymbol Getter(TypeSymbol containingSymbol, IdentifierName name, SelfParameterType selfParam, IType @return)
        => new(containingSymbol, MethodKind.Getter, name, selfParam, FixedList.Empty<ParameterType>(), @return);

    public static MethodSymbol Setter(TypeSymbol containingSymbol, IdentifierName name, SelfParameterType selfParam, ParameterType parameter)
        => new(containingSymbol, MethodKind.Setter, name, selfParam, FixedList.Create(parameter), IType.Void);
}
