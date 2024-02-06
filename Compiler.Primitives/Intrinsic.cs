using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.Primitives.SymbolBuilder;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Intrinsic
{
    public static readonly FixedSymbolTree SymbolTree = DefineIntrinsicSymbols();

    public static readonly ObjectTypeSymbol Promise = Find<ObjectTypeSymbol>("Promise");

    public static readonly DeclaredObjectType PromiseType = Promise.DeclaresType;

    public static ObjectType PromiseOf(DataType type)
        => PromiseType.WithRead(FixedList.Create(type));

    public static readonly ObjectTypeSymbol RawHybridBoundedList
        = Find<ObjectTypeSymbol>("Raw_Hybrid_Bounded_List");

    public static readonly ConstructorSymbol NewRawBoundedList
        = Find<ConstructorSymbol>(RawHybridBoundedList, null);

    public static readonly MethodSymbol GetRawBoundedListCapacity
        = Find<MethodSymbol>(RawHybridBoundedList, "get_capacity");

    public static readonly MethodSymbol GetRawBoundedListCount
        = Find<MethodSymbol>(RawHybridBoundedList, "get_count");

    public static readonly MethodSymbol RawBoundedListAdd
        = Find<MethodSymbol>(RawHybridBoundedList, "add");

    public static readonly MethodSymbol RawBoundedListAt
        = Find<MethodSymbol>(RawHybridBoundedList, "at");

    public static readonly MethodSymbol RawBoundedListSetAt
        = Find<MethodSymbol>(RawHybridBoundedList, "set_at");

    public static readonly MethodSymbol RawBoundedListShrink
        = Find<MethodSymbol>(RawHybridBoundedList, "shrink");

    public static readonly FunctionSymbol PrintRawUtf8Bytes = Find<FunctionSymbol>("print_raw_utf8_bytes");

    public static readonly FunctionSymbol AbortRawUtf8Bytes = Find<FunctionSymbol>("ABORT_RAW_UTF8_BYTES");

    public static readonly FunctionSymbol IdentityHash = Find<FunctionSymbol>("identity_hash");

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

        var azothNamespace = BuildAzothNamespace(intrinsicsPackage, tree);

        _ = BuildPromiseSymbol(azothNamespace, tree);

        var rawBoundedListType = BuildSpecializedCollectionSymbols(azothNamespace, tree);
        var readBytesType = rawBoundedListType.WithRead(FixedList.Create<DataType>(DataType.Void, DataType.Byte));

        // fn print_raw_utf8_bytes(bytes: Raw_Bounded_List[byte], start: size, byte_count: size)
        var print = Function(intrinsicsNamespace, "print_raw_utf8_bytes",
            Params(readBytesType, DataType.Size, DataType.Size));
        tree.Add(print);

        // fn read_raw_utf8_bytes_line(bytes: mut Raw_Bounded_List[byte], start: size) -> size
        var readLine = Function(intrinsicsNamespace, "read_raw_utf8_bytes_line",
            Params(DataType.Size, DataType.Size), ReturnType.Size);
        tree.Add(readLine);

        // fn ABORT_RAW_UTF8_BYTES(bytes: Raw_Bounded_List[byte], start: size, byte_count: size) -> never
        var abort = Function(intrinsicsNamespace, "ABORT_RAW_UTF8_BYTES",
            Params(readBytesType, DataType.Size, DataType.Size), ReturnType.Never);
        tree.Add(abort);

        var readAnyType = new AnyType(ReferenceCapability.ReadOnly);
        // fn identity_hash(value: Any) -> uint64 // TODO: should be nuint
        var identityHash = Function(intrinsicsNamespace, "identity_hash",
            Params(readAnyType), ReturnType.UInt64);
        tree.Add(identityHash);

        return tree.Build();
    }

    private static NamespaceSymbol BuildAzothNamespace(PackageSymbol intrinsicsPackage, SymbolTreeBuilder tree)
    {
        var azothNamespace = new NamespaceSymbol(intrinsicsPackage, "azoth");
        tree.Add(azothNamespace);
        return azothNamespace;
    }

    private static DeclaredObjectType BuildPromiseSymbol(NamespaceSymbol azothNamespace, SymbolTreeBuilder tree)
    {
        var intrinsicsPackage = azothNamespace.Package;
        var promiseType = DeclaredObjectType.Create(intrinsicsPackage.Name, azothNamespace.NamespaceName,
                       isAbstract: false, isConst: false, isClass: true, "Promise", GenericParameter.Out("T"));
        var classSymbol = new ObjectTypeSymbol(azothNamespace, promiseType);
        tree.Add(classSymbol);

        return promiseType;
    }

    private static DeclaredObjectType BuildSpecializedCollectionSymbols(
        NamespaceSymbol azothNamespace,
        SymbolTreeBuilder tree)
    {
        var collectionsNamespace = new NamespaceSymbol(azothNamespace, "collections");
        var specializedNamespace = new NamespaceSymbol(collectionsNamespace, "specialized");
        tree.Add(collectionsNamespace);
        tree.Add(specializedNamespace);

        return BuildRawHybridBoundedListSymbol(tree, specializedNamespace);
    }

    private static DeclaredObjectType BuildRawHybridBoundedListSymbol(SymbolTreeBuilder tree, NamespaceSymbol @namespace)
    {
        var classType = DeclaredObjectType.Create(@namespace.Package.Name, @namespace.NamespaceName,
            isAbstract: false, isConst: false, isClass: true, "Raw_Hybrid_Bounded_List",
            GenericParameter.Invariant("F"), GenericParameter.Invariant("T"));
        var fixedType = classType.GenericParameterTypes[0];
        var readClassParamType = new SelfParameterType(false, classType.WithRead(classType.GenericParameterDataTypes));
        var mutClassType = classType.WithMutate(classType.GenericParameterDataTypes);
        var mutClassParamType = new SelfParameterType(false, mutClassType);
        var itemType = classType.GenericParameterTypes[1];
        var classSymbol = new ObjectTypeSymbol(@namespace, classType);
        tree.Add(classSymbol);

        // published new(.fixed, .capacity) {...}
        var constructor = new ConstructorSymbol(classSymbol, null, mutClassType, Params(fixedType, DataType.Size));
        tree.Add(constructor);

        // published fn get_fixed() -> F;
        var getFixed = new MethodSymbol(classSymbol, "get_fixed", mutClassParamType, Params(), new ReturnType(false, fixedType));
        tree.Add(getFixed);

        // published fn set_fixed(fixed: F);
        var setFixed = new MethodSymbol(classSymbol, "set_fixed", mutClassParamType, Params(), ReturnType.Void);
        tree.Add(setFixed);

        // published fn get_capacity() -> size;
        var capacity = new MethodSymbol(classSymbol, "get_capacity", readClassParamType, Params(), ReturnType.Size);
        tree.Add(capacity);

        // Given setters are not implemented, making this a function for now
        // published fn get_count() -> size
        var count = new MethodSymbol(classSymbol, "get_count", readClassParamType, Params(), ReturnType.Size);
        tree.Add(count);

        // published /* unsafe */ fn at(self, index: size) -> T
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
}
