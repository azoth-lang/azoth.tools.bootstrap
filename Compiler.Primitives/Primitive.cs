using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using static Azoth.Tools.Bootstrap.Compiler.Primitives.SymbolBuilder;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Primitive
{
    public static readonly PrimitiveSymbolTree SymbolTree = DefinePrimitiveSymbols();

    public static readonly BuiltInTypeSymbol Any = Find<BuiltInTypeSymbol>("Any");

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

        BuildIntegerTypeSymbol(tree, TypeConstructor.Int, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.UInt, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.Int8, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.Byte, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.Int16, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.UInt16, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.Int32, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.UInt32, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.Int64, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.UInt64, stringType);

        BuildIntegerTypeSymbol(tree, TypeConstructor.Size, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.Offset, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.NInt, stringType);
        BuildIntegerTypeSymbol(tree, TypeConstructor.NUInt, stringType);

        BuildEmptyTypeSymbol(tree, IType.Void);
        BuildEmptyTypeSymbol(tree, IType.Never);

        BuildAnyTypeSymbol(tree);

        return tree.BuildPrimitives();
    }

    private static void BuildBoolSymbol(SymbolTreeBuilder tree)
    {
        var symbol = new BuiltInTypeSymbol(TypeConstructor.Bool);
        tree.Add(symbol);
    }

    private static void BuildIntegerTypeSymbol(
        SymbolTreeBuilder tree,
        IntegerTypeConstructor integerType,
        IType stringType)
    {
        var type = new BuiltInTypeSymbol(integerType);
        tree.Add(type);

        var integerParamType = SelfParam(integerType.ToType());

        // published fn remainder(self, other: T) -> T
        var remainderMethod = Method(type, "remainder", integerParamType, Params(integerType.ToType()), integerType.ToType());
        tree.Add(remainderMethod);

        // published fn to_display_string(self) -> String
        var displayStringMethod = Method(type, "to_display_string", integerParamType, Params(), stringType);
        tree.Add(displayStringMethod);
    }

    private static void BuildEmptyTypeSymbol(SymbolTreeBuilder tree, EmptyType emptyType)
    {
        var symbol = new EmptyTypeSymbol(emptyType);
        tree.Add(symbol);
    }

    private static void BuildAnyTypeSymbol(SymbolTreeBuilder tree)
    {
        var symbol = new BuiltInTypeSymbol(TypeConstructor.Any);
        tree.Add(symbol);

        var idAnyType = BareNonVariableType.Any.With(Capability.Identity);

        // published fn identity_hash(id self) -> nuint
        var identityHash = Method(symbol, "identity_hash", SelfParam(idAnyType), Params(), IType.NUInt);
        tree.Add(identityHash);
    }
}
