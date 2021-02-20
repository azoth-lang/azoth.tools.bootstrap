using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives
{
    public static class Primitive
    {
        public static readonly PrimitiveSymbolTree SymbolTree = DefinePrimitiveSymbols();

        private static PrimitiveSymbolTree DefinePrimitiveSymbols()
        {
            var tree = new SymbolTreeBuilder();

            var stringType = ObjectType.Create(NamespaceName.Global, "string", false);

            // Simple Types
            BuildBoolSymbol(tree);

            BuildIntegerTypeSymbol(tree, DataType.Byte, stringType);
            BuildIntegerTypeSymbol(tree, DataType.Int32, stringType);
            BuildIntegerTypeSymbol(tree, DataType.UInt32, stringType);

            BuildIntegerTypeSymbol(tree, DataType.Size, stringType);
            BuildIntegerTypeSymbol(tree, DataType.Offset, stringType);

            BuildEmptyType(tree, DataType.Void);
            BuildEmptyType(tree, DataType.Never);

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

            var remainderMethod = new MethodSymbol(type, "remainder", integerType, Params(integerType), integerType);
            var displayStringMethod = new MethodSymbol(type, "to_display_string", integerType, Params(), stringType);

            tree.Add(remainderMethod);
            tree.Add(displayStringMethod);
        }

        private static void BuildEmptyType(SymbolTreeBuilder tree, EmptyType emptyType)
        {
            var symbol = new PrimitiveTypeSymbol(emptyType);
            tree.Add(symbol);
        }

        private static FixedList<DataType> Params(params DataType[] types)
        {
            return types.ToFixedList();
        }
    }
}
