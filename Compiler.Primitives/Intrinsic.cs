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

    public static readonly FunctionSymbol PrintUtf8 = Find<FunctionSymbol>("print_utf8");

    public static readonly FunctionSymbol AbortUtf8 = Find<FunctionSymbol>("abort_utf8");

    public static readonly ObjectTypeSymbol RawBoundedList
        = Find<ObjectTypeSymbol>("Raw_Bounded_List");

    public static readonly ConstructorSymbol NewRawBoundedList
        = Find<ConstructorSymbol>(RawBoundedList, null);

    public static readonly MethodSymbol RawBoundedListCapacity
        = Find<MethodSymbol>(RawBoundedList, "capacity");

    public static readonly MethodSymbol RawBoundedListCount
        = Find<MethodSymbol>(RawBoundedList, "count");

    public static readonly MethodSymbol RawBoundedListAdd
        = Find<MethodSymbol>(RawBoundedList, "add");

    public static readonly MethodSymbol RawBoundedListAt
        = Find<MethodSymbol>(RawBoundedList, "at");

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

        // params: length
        var memAllocate = new FunctionSymbol(intrinsicsNamespace, "mem_allocate", Params(DataType.Size), DataType.Size);
        tree.Add(memAllocate);

        // params: ptr
        var memDeallocate = new FunctionSymbol(intrinsicsNamespace, "mem_deallocate", Params(DataType.Size));
        tree.Add(memDeallocate);

        // params: from_ptr, to_ptr, length
        var memCopy = new FunctionSymbol(intrinsicsNamespace, "mem_copy", Params(DataType.Size, DataType.Size, DataType.Size));
        tree.Add(memCopy);

        // params: from_ptr, value
        var memSetByte = new FunctionSymbol(intrinsicsNamespace, "mem_set_byte", Params(DataType.Size, DataType.Byte));
        tree.Add(memSetByte);

        // params: ptr
        var memGetByte = new FunctionSymbol(intrinsicsNamespace, "mem_get_byte", Params(DataType.Size), DataType.Byte);
        tree.Add(memGetByte);

        // params: ptr, length
        var printUtf8 = new FunctionSymbol(intrinsicsNamespace, "print_utf8", Params(DataType.Size, DataType.Size));
        tree.Add(printUtf8);

        // params: ptr, length
        var readUtf8Line = new FunctionSymbol(intrinsicsNamespace, "read_utf8_line", Params(DataType.Size, DataType.Size), DataType.Size);
        tree.Add(readUtf8Line);

        var abortUtf8 = new FunctionSymbol(intrinsicsNamespace, "abort_utf8", Params(DataType.Size, DataType.Size), DataType.Never);
        tree.Add(abortUtf8);

        BuildSpecializedCollectionSymbols(intrinsicsPackage, tree);


        return tree.Build();
    }

    private static void BuildSpecializedCollectionSymbols(
        PackageSymbol intrinsicsPackage,
        SymbolTreeBuilder tree)
    {
        var azothNamespace = new NamespaceSymbol(intrinsicsPackage, "azoth");
        var collectionsNamespace = new NamespaceSymbol(intrinsicsPackage, "collections");
        var specializedNamespace = new NamespaceSymbol(intrinsicsPackage, "specialized");
        tree.Add(azothNamespace);
        tree.Add(collectionsNamespace);
        tree.Add(specializedNamespace);

        BuildRawBoundedListSymbol(tree, specializedNamespace);
    }

    private static void BuildRawBoundedListSymbol(SymbolTreeBuilder tree, NamespaceSymbol @namespace)
    {
        var classType = DeclaredObjectType.Create(@namespace.Package!.Name, @namespace.NamespaceName, "Raw_Bounded_List", false, "T");
        var readClassType = classType.WithRead(classType.GenericParameterDataTypes);
        var itemType = classType.GenericParameterTypes[0];
        var classSymbol = new ObjectTypeSymbol(@namespace, classType);
        tree.Add(classSymbol);

        // published new(.capacity) {...}
        var constructor = new ConstructorSymbol(classSymbol, null, Params(DataType.Size));
        tree.Add(constructor);

        // published fn capacity() -> size;
        var capacity = new MethodSymbol(classSymbol, "capacity", readClassType, Params(), DataType.Size);
        tree.Add(capacity);

        // Given setters are not implemented, making this a function for now
        // published fn count() -> size
        var count = new MethodSymbol(classSymbol, "count", readClassType, Params(), DataType.Size);
        tree.Add(count);

        // published /* unsafe */ fn at(mut self, index: size) -> T
        var at = new MethodSymbol(classSymbol, "at", readClassType, Params(DataType.Size), itemType);
        tree.Add(at);

        // published /* unsafe */ fn set(mut self, index: size, T value)
        var set = new MethodSymbol(classSymbol, "set", readClassType, Params(DataType.Size, itemType),
            DataType.Void);
        tree.Add(set);

        // published fn add(mut self, value: T);
        var add = new MethodSymbol(classSymbol, "add", readClassType, Params(itemType), DataType.Void);
        tree.Add(add);
    }

    private static FixedList<DataType> Params(params DataType[] types) => types.ToFixedList();
}
