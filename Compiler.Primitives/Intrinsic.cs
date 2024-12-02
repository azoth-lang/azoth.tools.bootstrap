using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.Primitives.SymbolBuilder;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class Intrinsic
{
    public static readonly FixedSymbolTree SymbolTree = DefineIntrinsicSymbols();

    public static readonly PackageSymbol Package = SymbolTree.Package;

    public static readonly OrdinaryTypeSymbol Promise = Find<OrdinaryTypeSymbol>("Promise");

    public static readonly IDeclaredUserType PromiseDeclaredType = Promise.DeclaresType;
    public static readonly ITypeConstructor PromiseTypeConstructor = PromiseDeclaredType.ToTypeConstructor();

    public static IMaybeExpressionType PromiseOf(IMaybeType type)
        => PromiseDeclaredType.WithRead(FixedList.Create(type));
    public static IMaybePlainType PromiseOf(IMaybePlainType plainType)
        => PromiseTypeConstructor.Construct(FixedList.Create(plainType));

    public static readonly OrdinaryTypeSymbol RawHybridBoundedList
        = Find<OrdinaryTypeSymbol>("Raw_Hybrid_Bounded_List");

    public static readonly ConstructorSymbol NewRawBoundedList
        = Find<ConstructorSymbol>(RawHybridBoundedList, null);

    public static readonly MethodSymbol GetRawBoundedListCapacity
        = Find<MethodSymbol>(RawHybridBoundedList, "capacity");

    public static readonly MethodSymbol GetRawBoundedListCount
        = Find<MethodSymbol>(RawHybridBoundedList, "count");

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
        var readBytesType = rawHybridBoundedListType.WithRead([IType.Void, IType.Byte]);

        // fn print_raw_utf8_bytes(bytes: Raw_Hybrid_Bounded_List[byte], start: size, byte_count: size)
        var print = Function(intrinsicsNamespace, "print_raw_utf8_bytes",
            Params(readBytesType, IType.Size, IType.Size));
        tree.Add(print);

        // fn read_raw_utf8_bytes_line(bytes: mut Raw_Hybrid_Bounded_List[byte], start: size) -> size
        var readLine = Function(intrinsicsNamespace, "read_raw_utf8_bytes_line",
            Params(IType.Size, IType.Size), IType.Size);
        tree.Add(readLine);

        // fn ABORT_RAW_UTF8_BYTES(bytes: Raw_Hybrid_Bounded_List[byte], start: size, byte_count: size) -> never
        var abort = Function(intrinsicsNamespace, "ABORT_RAW_UTF8_BYTES",
            Params(readBytesType, IType.Size, IType.Size), IType.Never);
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
        var classSymbol = new OrdinaryTypeSymbol(azothNamespace, promiseType);
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
        var classSymbol = new OrdinaryTypeSymbol(@namespace, classType);
        tree.Add(classSymbol);

        // published new(.fixed, .capacity) {...}
        var constructor = new ConstructorSymbol(classSymbol, null, mutClassType, Params(fixedType, IType.Size));
        tree.Add(constructor);

        // published get fixed(self) -> F;
        var getFixed = Getter(classSymbol, "fixed", readClassParamType, fixedType);
        tree.Add(getFixed);

        // published set fixed(mut self, fixed: F);
        var setFixed = Setter(classSymbol, "fixed", mutClassParamType, Param(fixedType));
        tree.Add(setFixed);

        // published get capacity(self) -> size;
        var capacity = Getter(classSymbol, "capacity", readClassParamType, IType.Size);
        tree.Add(capacity);

        // published get count(self) -> size
        var count = Getter(classSymbol, "count", readClassParamType, IType.Size);
        tree.Add(count);

        // published /* unsafe */ fn at(self, index: size) -> T
        // TODO replace with at method returning a `ref var`
        var at = Method(classSymbol, "at", readClassParamType,
            Params(IType.Size), itemType);
        tree.Add(at);

        // published /* unsafe */ fn set_at(mut self, index: size, T value)
        // TODO replace with at method returning a `ref var`
        var setAt = Method(classSymbol, "set_at", mutClassParamType, Params(IType.Size, itemType));
        tree.Add(setAt);

        // published fn add(mut self, value: T);
        var add = Method(classSymbol, "add", mutClassParamType, Params(itemType));
        tree.Add(add);

        // published fn shrink(mut self, count: size)
        var shrink = Method(classSymbol, "shrink", mutClassParamType, Params(IType.Size));
        tree.Add(shrink);

        return classType;
    }
}
