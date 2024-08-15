using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using static Azoth.Tools.Bootstrap.Compiler.Primitives.SymbolBuilder;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Primitive
{
    public static readonly PrimitiveSymbolTree SymbolTree = DefinePrimitiveSymbols();

    public static readonly PrimitiveTypeSymbol Any = Find<PrimitiveTypeSymbol>("Any");

    public static readonly MethodSymbol IdentityHash = Find<MethodSymbol>(Any, "identity_hash");

    private static IEnumerable<T> Find<T>() => SymbolTree.Symbols.OfType<T>();

    private static T Find<T>(string name)
        where T : Symbol
        => Find<T>().Single(s => s.Name?.Text == name);

    private static T Find<T>(Symbol containingSymbol, string? name)
        where T : Symbol
        => Find<T>().Single(s => s.ContainingSymbol == containingSymbol && s.Name?.Text == name);

    private static PrimitiveSymbolTree DefinePrimitiveSymbols()
    {
        var tree = SymbolTreeBuilder.CreateForPrimitives();

        // TODO: This is a hack to "have" a string type from here. Replace by extending primitive types with string related methods.
        var stringType = CapabilityType.CreateClass(Capability.Constant, "fake", NamespaceName.Global, false, false, "String");

        // Simple Types
        BuildBoolSymbol(tree);

        BuildIntegerTypeSymbol(tree, DeclaredType.Int, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.UInt, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.Int8, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.Byte, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.Int16, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.UInt16, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.Int32, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.UInt32, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.Int64, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.UInt64, stringType);

        BuildIntegerTypeSymbol(tree, DeclaredType.Size, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.Offset, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.NInt, stringType);
        BuildIntegerTypeSymbol(tree, DeclaredType.NUInt, stringType);

        BuildEmptyTypeSymbol(tree, DataType.Void);
        BuildEmptyTypeSymbol(tree, DataType.Never);

        BuildAnyTypeSymbol(tree);

        return tree.BuildPrimitives();
    }

    private static void BuildBoolSymbol(SymbolTreeBuilder tree)
    {
        var symbol = new PrimitiveTypeSymbol(DeclaredType.Bool);
        tree.Add(symbol);
    }

    private static void BuildIntegerTypeSymbol(
        SymbolTreeBuilder tree,
        IntegerType integerType,
        DataType stringType)
    {
        var type = new PrimitiveTypeSymbol(integerType);
        tree.Add(type);

        var integerParamType = SelfParam(integerType.Type);

        var remainderMethod = new MethodSymbol(type, "remainder", integerParamType, Params(integerType.Type), Return(integerType.Type));
        tree.Add(remainderMethod);

        var displayStringMethod = new MethodSymbol(type, "to_display_string", integerParamType, Params(), Return(stringType));
        tree.Add(displayStringMethod);
    }

    private static void BuildEmptyTypeSymbol(SymbolTreeBuilder tree, EmptyType emptyType)
    {
        var symbol = new EmptyTypeSymbol(emptyType);
        tree.Add(symbol);
    }

    private static void BuildAnyTypeSymbol(SymbolTreeBuilder tree)
    {
        var symbol = new PrimitiveTypeSymbol(DeclaredType.Any);
        tree.Add(symbol);

        var idAnyType = DeclaredType.Any.With(Capability.Identity);
        // fn identity_hash(value: Any) -> nuint
        var identityHash = new MethodSymbol(symbol, "identity_hash", SelfParam(idAnyType), Params(), ReturnType.NUInt);
        tree.Add(identityHash);
    }
}
