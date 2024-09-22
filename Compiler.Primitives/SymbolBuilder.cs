using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

internal static class SymbolBuilder
{
    public static SelfParameterType SelfParam(INonVoidType type) => new(false, type);

    public static IFixedList<ParameterType> Params(params INonVoidType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    public static IType Return(IType type) => type;

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params)
        => new(containingSymbol, name, new FunctionType(@params, IType.Void));

    public static FunctionSymbol Function(Symbol containingSymbol, IdentifierName name, IFixedList<ParameterType> @params, IType @return)
        => new(containingSymbol, name, new FunctionType(@params, @return));
}
