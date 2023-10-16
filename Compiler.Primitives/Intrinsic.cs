using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Intrinsic
{
    public static readonly FixedSymbolTree SymbolTree = DefineIntrinsicSymbols();

    public static readonly FunctionSymbol PrintRawUtf8Bytes = Find<FunctionSymbol>("print_raw_utf8_bytes");

    public static readonly FunctionSymbol AbortRawUtf8Bytes = Find<FunctionSymbol>("ABORT_RAW_UTF8_BYTES");

    public static readonly ObjectTypeSymbol RawBoundedList
        = Find<ObjectTypeSymbol>("Raw_Bounded_List");

    public static readonly ConstructorSymbol NewRawBoundedList
        = Find<ConstructorSymbol>(RawBoundedList, null);

    public static readonly MethodSymbol GetRawBoundedListCapacity
        = Find<MethodSymbol>(RawBoundedList, "get_capacity");

    public static readonly MethodSymbol GetRawBoundedListCount
        = Find<MethodSymbol>(RawBoundedList, "get_count");

    public static readonly MethodSymbol RawBoundedListAdd
        = Find<MethodSymbol>(RawBoundedList, "add");

    public static readonly MethodSymbol RawBoundedListAt
        = Find<MethodSymbol>(RawBoundedList, "at");

    public static readonly MethodSymbol RawBoundedListSetAt
        = Find<MethodSymbol>(RawBoundedList, "set_at");

    public static readonly MethodSymbol RawBoundedListShrink
        = Find<MethodSymbol>(RawBoundedList, "shrink");

    private static IEnumerable<T> Find<T>()
        => SymbolTree.Symbols.OfType<T>();

    private static T Find<T>(string name)
        where T : Symbol
        => Find<T>().Single(s => s.Name?.Text == name);

    private static T Find<T>(Symbol containingSymbol, string? name)
        where T : Symbol =>
        Find<T>().Single(s => s.ContainingSymbol == containingSymbol && s.Name?.Text == name);

    private static FixedSymbolTree DefineIntrinsicSymbols()
    {
        var intrinsicsPackage = new PackageSymbol("intrinsics");
        var tree = new SymbolTreeBuilder(intrinsicsPackage);

        var intrinsicsNamespace = new NamespaceSymbol(intrinsicsPackage, "intrinsics");

        var rawBoundedListType = BuildSpecializedCollectionSymbols(intrinsicsPackage, tree);
        var readBytesType = rawBoundedListType.WithRead(DataType.Byte.Yield().ToFixedList<DataType>());

        // fn print_raw_utf8_bytes(bytes: Raw_Bounded_List[byte], start: size, byte_count: size)
        var print = new FunctionSymbol(intrinsicsNamespace, "print_raw_utf8_bytes",
            Params(readBytesType, DataType.Size, DataType.Size));
        tree.Add(print);

        // fn read_raw_utf8_bytes_line(bytes: mut Raw_Bounded_List[byte], start: size) -> size
        var readLine = new FunctionSymbol(intrinsicsNamespace, "read_raw_utf8_bytes_line",
            Params(DataType.Size, DataType.Size), ReturnType.Size);
        tree.Add(readLine);

        // fn ABORT_RAW_UTF8_BYTES(bytes: Raw_Bounded_List[byte], start: size, byte_count: size) -> never
        var abort = new FunctionSymbol(intrinsicsNamespace, "ABORT_RAW_UTF8_BYTES",
            Params(readBytesType, DataType.Size, DataType.Size), ReturnType.Never);
        tree.Add(abort);

        return tree.Build();
    }

    private static DeclaredObjectType BuildSpecializedCollectionSymbols(
        PackageSymbol intrinsicsPackage,
        SymbolTreeBuilder tree)
    {
        var azothNamespace = new NamespaceSymbol(intrinsicsPackage, "azoth");
        var collectionsNamespace = new NamespaceSymbol(intrinsicsPackage, "collections");
        var specializedNamespace = new NamespaceSymbol(intrinsicsPackage, "specialized");
        tree.Add(azothNamespace);
        tree.Add(collectionsNamespace);
        tree.Add(specializedNamespace);

        return BuildRawBoundedListSymbol(tree, specializedNamespace);
    }

    private static DeclaredObjectType BuildRawBoundedListSymbol(SymbolTreeBuilder tree, NamespaceSymbol @namespace)
    {
        var classType = DeclaredObjectType.Create(@namespace.Package!.Name, @namespace.NamespaceName,
            isAbstract: false, isConst: false, isClass: true, "Raw_Bounded_List", "T");
        var readClassParamType = new ParameterType(false, classType.WithRead(classType.GenericParameterDataTypes));
        var mutClassType = classType.WithMutate(classType.GenericParameterDataTypes);
        var mutClassParamType = new ParameterType(false, mutClassType);
        var itemType = classType.GenericParameterTypes[0];
        var classSymbol = new ObjectTypeSymbol(@namespace, classType);
        tree.Add(classSymbol);

        // published new(.capacity) {...}
        var constructor = new ConstructorSymbol(classSymbol, null, mutClassType, Params(DataType.Size));
        tree.Add(constructor);

        // published fn get_capacity() -> size;
        var capacity = new MethodSymbol(classSymbol, "get_capacity", readClassParamType, Params(), ReturnType.Size);
        tree.Add(capacity);

        // Given setters are not implemented, making this a function for now
        // published fn get_count() -> size
        var count = new MethodSymbol(classSymbol, "get_count", readClassParamType, Params(), ReturnType.Size);
        tree.Add(count);

        // published /* unsafe */ fn at(mut self, index: size) -> T
        var at = new MethodSymbol(classSymbol, "at", readClassParamType,
            Params(DataType.Size), Return(itemType));
        tree.Add(at);

        // published /* unsafe */ fn set_at(mut self, index: size, T value)
        // TODO replace with at method returning a `ref var`
        var setAt = new MethodSymbol(classSymbol, "set_at", mutClassParamType,
            Params(DataType.Size, itemType), ReturnType.Void);
        tree.Add(setAt);

        // published fn add(mut self, value: T);
        var add = new MethodSymbol(classSymbol, "add", mutClassParamType,
            Params(itemType), ReturnType.Void);
        tree.Add(add);

        // published fn shrink(mut self, count: size)
        var shrink = new MethodSymbol(classSymbol, "shrink", mutClassParamType,
            Params(DataType.Size), ReturnType.Void);
        tree.Add(shrink);

        return classType;
    }

    private static FixedList<ParameterType> Params(params DataType[] types)
        => types.Select(t => new ParameterType(false, t)).ToFixedList();

    private static ReturnType Return(DataType type) => new(false, type);
}
