using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Primitive
{
    public static readonly PrimitiveSymbolTree SymbolTree = DefinePrimitiveSymbols();

    private static PrimitiveSymbolTree DefinePrimitiveSymbols()
    {
        var tree = SymbolTreeBuilder.CreateForPrimitives();

        // TODO: This is a hack to "have" a string type from here. Replace by extending primitive types with string related methods.
        var stringType = ObjectType.Create(ReferenceCapability.Constant, "", NamespaceName.Global, false, false, true, "String");

        // Simple Types
        BuildBoolSymbol(tree);

        BuildIntegerTypeSymbol(tree, DataType.Int, stringType);
        BuildIntegerTypeSymbol(tree, DataType.UInt, stringType);
        BuildIntegerTypeSymbol(tree, DataType.Int8, stringType);
        BuildIntegerTypeSymbol(tree, DataType.Byte, stringType);
        BuildIntegerTypeSymbol(tree, DataType.Int16, stringType);
        BuildIntegerTypeSymbol(tree, DataType.UInt16, stringType);
        BuildIntegerTypeSymbol(tree, DataType.Int32, stringType);
        BuildIntegerTypeSymbol(tree, DataType.UInt32, stringType);
        BuildIntegerTypeSymbol(tree, DataType.Int64, stringType);
        BuildIntegerTypeSymbol(tree, DataType.UInt64, stringType);

        BuildIntegerTypeSymbol(tree, DataType.Size, stringType);
        BuildIntegerTypeSymbol(tree, DataType.Offset, stringType);

        BuildEmptyTypeSymbol(tree, DataType.Void);
        BuildEmptyTypeSymbol(tree, DataType.Never);

        BuildAnyTypeSymbol(tree);

        return tree.BuildPrimitives();
    }

    private static void BuildBoolSymbol(SymbolTreeBuilder tree)
    {
        var symbol = new PrimitiveTypeSymbol(DataType.Bool);
        tree.Add(symbol);
    }

    private static void BuildIntegerTypeSymbol(
        SymbolTreeBuilder tree,
        IntegerType integerType,
        DataType stringType)
    {
        var type = new PrimitiveTypeSymbol(integerType);
        tree.Add(type);

        var integerParamType = new ParameterType(false, integerType);
        var remainderMethod = new MethodSymbol(type, "remainder", integerParamType, Params(integerType), Return(integerType));
        var displayStringMethod = new MethodSymbol(type, "to_display_string", integerParamType, Params(), Return(stringType));

        tree.Add(remainderMethod);
        tree.Add(displayStringMethod);
    }

    private static void BuildEmptyTypeSymbol(SymbolTreeBuilder tree, EmptyType emptyType)
    {
        var symbol = new PrimitiveTypeSymbol(emptyType);
        tree.Add(symbol);
    }

    private static void BuildAnyTypeSymbol(SymbolTreeBuilder tree)
    {
        var symbol = new PrimitiveTypeSymbol(DeclaredReferenceType.Any.With(ReferenceCapability.Mutable));
        tree.Add(symbol);
    }

    private static FixedList<ParameterType> Params(params DataType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    private static ReturnType Return(DataType type) => new(false, type);
}
