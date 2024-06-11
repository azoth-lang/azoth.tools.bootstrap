using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.Primitives.SymbolBuilder;
using Return = Azoth.Tools.Bootstrap.Compiler.Types.Return;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Intrinsic
{
    public static readonly FixedSymbolTree SymbolTree = DefineIntrinsicSymbols();

    public static readonly PackageSymbol Package = SymbolTree.Package;

    public static readonly UserTypeSymbol Promise = Find<UserTypeSymbol>("Promise");

    public static readonly IDeclaredUserType PromiseType = Promise.DeclaresType;
    public static readonly IDeclaredAntetype PromiseAntetype = PromiseType.ToAntetype();

    public static CapabilityType PromiseOf(DataType type)
        => PromiseType.WithRead(FixedList.Create(type));
    public static IMaybeAntetype PromiseOf(IMaybeExpressionAntetype antetype)
    {
        if (antetype.ToNonConstValueType() is not IAntetype knownAntetype)
            return IAntetype.Unknown;
        return PromiseAntetype.With(FixedList.Create(knownAntetype));
    }

    public static readonly UserTypeSymbol RawHybridBoundedList
        = Find<UserTypeSymbol>("Raw_Hybrid_Bounded_List");

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

    private static IEnumerable<T> Find<T>()
        => SymbolTree.Symbols.OfType<T>();

    private static T Find<T>(string name)
        where T : Symbol
        => Find<T>().Single(s => s.Name?.Text == name);

    private static T Find<T>(Symbol containingSymbol, string? name)
        where T : Symbol
        => Find<T>().Single(s => s.ContainingSymbol == containingSymbol && s.Name?.Text == name);

    private static FixedSymbolTree DefineIntrinsicSymbols()
    {
        var intrinsicsPackage = new PackageSymbol("azoth.compiler.intrinsics");
        var tree = new SymbolTreeBuilder(intrinsicsPackage);

        var intrinsicsNamespace = new LocalNamespaceSymbol(intrinsicsPackage, "intrinsics");

        var azothNamespace = BuildAzothNamespace(intrinsicsPackage, tree);

        _ = BuildPromiseSymbol(azothNamespace, tree);

        var rawHybridBoundedListType = BuildSpecializedCollectionSymbols(azothNamespace, tree);
        var readBytesType = rawHybridBoundedListType.WithRead(FixedList.Create<DataType>(DataType.Void, DataType.Byte));

        // fn print_raw_utf8_bytes(bytes: Raw_Hybrid_Bounded_List[byte], start: size, byte_count: size)
        var print = Function(intrinsicsNamespace, "print_raw_utf8_bytes",
            Params(readBytesType, DataType.Size, DataType.Size));
        tree.Add(print);

        // fn read_raw_utf8_bytes_line(bytes: mut Raw_Hybrid_Bounded_List[byte], start: size) -> size
        var readLine = Function(intrinsicsNamespace, "read_raw_utf8_bytes_line",
            Params(DataType.Size, DataType.Size), Return.Size);
        tree.Add(readLine);

        // fn ABORT_RAW_UTF8_BYTES(bytes: Raw_Hybrid_Bounded_List[byte], start: size, byte_count: size) -> never
        var abort = Function(intrinsicsNamespace, "ABORT_RAW_UTF8_BYTES",
            Params(readBytesType, DataType.Size, DataType.Size), Return.Never);
        tree.Add(abort);

        return tree.Build();
    }

    private static LocalNamespaceSymbol BuildAzothNamespace(PackageSymbol intrinsicsPackage, SymbolTreeBuilder tree)
    {
        var azothNamespace = new LocalNamespaceSymbol(intrinsicsPackage, "azoth");
        tree.Add(azothNamespace);
        return azothNamespace;
    }

    private static ObjectType BuildPromiseSymbol(LocalNamespaceSymbol azothNamespace, SymbolTreeBuilder tree)
    {
        var intrinsicsPackage = azothNamespace.Package;
        var promiseType = ObjectType.CreateClass(intrinsicsPackage.Name, azothNamespace.NamespaceName,
                       isAbstract: false, isConst: false, "Promise", GenericParameter.Out(CapabilitySet.Any, "T"));
        var classSymbol = new UserTypeSymbol(azothNamespace, promiseType);
        tree.Add(classSymbol);

        return promiseType;
    }

    private static ObjectType BuildSpecializedCollectionSymbols(
        LocalNamespaceSymbol azothNamespace,
        SymbolTreeBuilder tree)
    {
        var collectionsNamespace = new LocalNamespaceSymbol(azothNamespace, "collections");
        var specializedNamespace = new LocalNamespaceSymbol(collectionsNamespace, "specialized");
        tree.Add(collectionsNamespace);
        tree.Add(specializedNamespace);

        return BuildRawHybridBoundedListSymbol(tree, specializedNamespace);
    }

    private static ObjectType BuildRawHybridBoundedListSymbol(SymbolTreeBuilder tree, LocalNamespaceSymbol @namespace)
    {
        var classType = ObjectType.CreateClass(@namespace.Package.Name, @namespace.NamespaceName,
            isAbstract: false, isConst: false, "Raw_Hybrid_Bounded_List",
            GenericParameter.Independent(CapabilitySet.Aliasable, "F"), GenericParameter.Independent(CapabilitySet.Aliasable, "T"));
        var fixedType = classType.GenericParameterTypes[0];
        var readClassParamType = new SelfParameterType(false, classType.WithRead(classType.GenericParameterTypes));
        var mutClassType = classType.WithMutate(classType.GenericParameterTypes);
        var mutClassParamType = new SelfParameterType(false, mutClassType);
        var itemType = classType.GenericParameterTypes[1];
        var classSymbol = new UserTypeSymbol(@namespace, classType);
        tree.Add(classSymbol);

        // published new(.fixed, .capacity) {...}
        var constructor = new ConstructorSymbol(classSymbol, null, mutClassType, Params(fixedType, DataType.Size));
        tree.Add(constructor);

        // published fn get_fixed() -> F;
        var getFixed = new MethodSymbol(classSymbol, "get_fixed", mutClassParamType, Params(), Return(fixedType));
        tree.Add(getFixed);

        // published fn set_fixed(fixed: F);
        var setFixed = new MethodSymbol(classSymbol, "set_fixed", mutClassParamType, Params(), Return.Void);
        tree.Add(setFixed);

        // published fn get_capacity() -> size;
        var capacity = new MethodSymbol(classSymbol, "get_capacity", readClassParamType, Params(), Return.Size);
        tree.Add(capacity);

        // Given setters are not implemented, making this a function for now
        // published fn get_count() -> size
        var count = new MethodSymbol(classSymbol, "get_count", readClassParamType, Params(), Return.Size);
        tree.Add(count);

        // published /* unsafe */ fn at(self, index: size) -> T
        var at = new MethodSymbol(classSymbol, "at", readClassParamType,
            Params(DataType.Size), Return(itemType));
        tree.Add(at);

        // published /* unsafe */ fn set_at(mut self, index: size, T value)
        // TODO replace with at method returning a `ref var`
        var setAt = new MethodSymbol(classSymbol, "set_at", mutClassParamType,
            Params(DataType.Size, itemType), Return.Void);
        tree.Add(setAt);

        // published fn add(mut self, value: T);
        var add = new MethodSymbol(classSymbol, "add", mutClassParamType,
            Params(itemType), Return.Void);
        tree.Add(add);

        // published fn shrink(mut self, count: size)
        var shrink = new MethodSymbol(classSymbol, "shrink", mutClassParamType,
            Params(DataType.Size), Return.Void);
        tree.Add(shrink);

        return classType;
    }
}
