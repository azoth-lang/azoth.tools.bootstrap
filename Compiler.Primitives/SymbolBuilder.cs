using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

internal static class SymbolBuilder
{
    public static SelfParameterType SelfParam(DataType type) => new(false, type);

    public static IFixedList<ParameterType> Params(params DataType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    public static ReturnType Return(DataType type) => new(type);

    public static FunctionSymbol Function(Symbol containingSymbol, SimpleName name, IFixedList<ParameterType> @params)
        => new(containingSymbol, name, new FunctionType(@params, ReturnType.Void));

    public static FunctionSymbol Function(Symbol containingSymbol, SimpleName name, IFixedList<ParameterType> @params, ReturnType @return)
        => new(containingSymbol, name, new FunctionType(@params, @return));
}
