using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

internal static class SymbolBuilder
{
    public static SelfParameter SelfParam(DataType type) => new(false, type);

    public static IFixedList<Parameter> Params(params DataType[] types)
        => types.Select(t => new Parameter(false, t)).ToFixedList();

    public static Return Return(DataType type) => new(type);

    public static FunctionSymbol Function(Symbol containingSymbol, SimpleName name, IFixedList<Parameter> @params)
        => new(containingSymbol, name, new FunctionType(@params, Types.Return.Void));

    public static FunctionSymbol Function(Symbol containingSymbol, SimpleName name, IFixedList<Parameter> @params, Return @return)
        => new(containingSymbol, name, new FunctionType(@params, @return));
}
