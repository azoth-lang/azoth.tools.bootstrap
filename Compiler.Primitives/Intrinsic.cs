using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.Symbols.SymbolBuilder;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

// TODO these should be moved to new style intrinsics that are declared in azoth.compiler.intrinsics with #Intrinsic
public static class Intrinsic
{
    public static readonly FixedSymbolTree SymbolTree = DefineIntrinsicSymbols();

    public static readonly PackageSymbol Package = SymbolTree.Package;

    public static readonly OrdinaryTypeSymbol Promise = Find<OrdinaryTypeSymbol>("Promise");

    public static readonly BareTypeConstructor PromiseTypeConstructor = Promise.TypeConstructor;

    public static IMaybeType PromiseOf(IMaybeType type)
        => PromiseTypeConstructor.TryConstructBareType(containingType: null, FixedList.Create(type))?.WithDefaultCapability() ?? IMaybeType.Unknown;
    public static IMaybePlainType PromiseOf(IMaybePlainType plainType)
        => PromiseTypeConstructor.Construct(containingType: null, FixedList.Create(plainType));

    public static readonly OrdinaryTypeSymbol RawHybridBoundedList
        = Find<OrdinaryTypeSymbol>("Raw_Hybrid_Bounded_List");

    public static readonly InitializerSymbol InitRawHybridBoundedList
        = Find<InitializerSymbol>(RawHybridBoundedList, null);

    public static readonly MethodSymbol GetRawHybridBoundedListCapacity
        = Find<MethodSymbol>(RawHybridBoundedList, "capacity");

    public static readonly MethodSymbol GetRawHybridBoundedListCount
        = Find<MethodSymbol>(RawHybridBoundedList, "count");

    public static readonly MethodSymbol RawHybridBoundedListAdd
        = Find<MethodSymbol>(RawHybridBoundedList, "add");

    public static readonly MethodSymbol RawHybridBoundedListAt
        = Find<MethodSymbol>(RawHybridBoundedList, "at");

    public static readonly MethodSymbol RawHybridBoundedListShrink
        = Find<MethodSymbol>(RawHybridBoundedList, "shrink");

    public static readonly MethodSymbol GetRawHybridBoundedPrefix
        = FindGetter(RawHybridBoundedList, "prefix");

    public static readonly MethodSymbol SetRawHybridBoundedPrefix
        = FindSetter(RawHybridBoundedList, "prefix");

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

    private static MethodSymbol FindGetter(Symbol containingSymbol, string name)
        => Find<MethodSymbol>().Single(s => s.Kind == MethodKind.Getter && s.ContainingSymbol == containingSymbol && s.Name.Text == name);

    private static MethodSymbol FindSetter(Symbol containingSymbol, string name)
        => Find<MethodSymbol>().Single(s => s.Kind == MethodKind.Setter && s.ContainingSymbol == containingSymbol && s.Name.Text == name);

    private static FixedSymbolTree DefineIntrinsicSymbols()
    {
        var intrinsicsPackage = new PackageSymbol("azoth.compiler.intrinsics.internal");
        var intrinsicsFacet = new PackageFacetSymbol(intrinsicsPackage, FacetKind.Main);
        var tree = new SymbolTreeBuilder(intrinsicsFacet);

        var intrinsicsNamespace = new LocalNamespaceSymbol(intrinsicsFacet, "intrinsics");

        var azothNamespace = BuildAzothNamespace(intrinsicsFacet, tree);

        _ = BuildPromiseSymbol(azothNamespace, tree);

        var rawHybridBoundedListType = BuildSpecializedCollectionSymbols(azothNamespace, tree);
        var readBytesType = rawHybridBoundedListType.Construct(containingType: null, [Type.Void, Type.Byte]).WithDefaultCapability();

        // fn print_raw_utf8_bytes(bytes: Raw_Hybrid_Bounded_List[byte], start: size, byte_count: size)
        var print = Function(intrinsicsNamespace, "print_raw_utf8_bytes",
            Params(readBytesType, Type.Size, Type.Size));
        tree.Add(print);

        // fn read_raw_utf8_bytes_line(bytes: mut Raw_Hybrid_Bounded_List[byte], start: size) -> size
        var readLine = Function(intrinsicsNamespace, "read_raw_utf8_bytes_line",
            Params(Type.Size, Type.Size), Type.Size);
        tree.Add(readLine);

        // fn ABORT_RAW_UTF8_BYTES(bytes: Raw_Hybrid_Bounded_List[byte], start: size, byte_count: size) -> never
        var abort = Function(intrinsicsNamespace, "ABORT_RAW_UTF8_BYTES",
            Params(readBytesType, Type.Size, Type.Size), Type.Never);
        tree.Add(abort);

        return tree.Build();
    }

    private static LocalNamespaceSymbol BuildAzothNamespace(PackageFacetSymbol intrinsicsFacet, SymbolTreeBuilder tree)
    {
        var azothNamespace = new LocalNamespaceSymbol(intrinsicsFacet, "azoth");
        tree.Add(azothNamespace);
        return azothNamespace;
    }

    private static OrdinaryTypeConstructor BuildPromiseSymbol(LocalNamespaceSymbol azothNamespace, SymbolTreeBuilder tree)
    {
        var intrinsicsPackage = azothNamespace.Package;
        var promiseType = BareTypeConstructor.CreateClass(intrinsicsPackage.Name, azothNamespace.NamespaceName,
                       isAbstract: false, isConst: false, "Promise", TypeConstructorParameter.Out(CapabilitySet.Any, "T"));
        var classSymbol = new OrdinaryTypeSymbol(azothNamespace, promiseType);
        tree.Add(classSymbol);

        return promiseType;
    }

    private static OrdinaryTypeConstructor BuildSpecializedCollectionSymbols(
        LocalNamespaceSymbol azothNamespace,
        SymbolTreeBuilder tree)
    {
        var collectionsNamespace = new LocalNamespaceSymbol(azothNamespace, "collections");
        var specializedNamespace = new LocalNamespaceSymbol(collectionsNamespace, "specialized");
        tree.Add(collectionsNamespace);
        tree.Add(specializedNamespace);

        return BuildRawHybridBoundedListSymbol(tree, specializedNamespace);
    }

    private static OrdinaryTypeConstructor BuildRawHybridBoundedListSymbol(SymbolTreeBuilder tree, LocalNamespaceSymbol @namespace)
    {
        var typeConstructor = BareTypeConstructor.CreateClass(@namespace.Package.Name, @namespace.NamespaceName,
            isAbstract: false, isConst: false, "Raw_Hybrid_Bounded_List",
            TypeConstructorParameter.Independent(CapabilitySet.Aliasable, "P"),
            TypeConstructorParameter.Independent(CapabilitySet.Aliasable, "T"));
        var plainType = typeConstructor.ConstructWithParameterPlainTypes();
        var bareType = typeConstructor.ConstructWithParameterTypes(plainType);
        var bareSelfType = BareSelfType(bareType);
        var readableSelfType = new CapabilitySetSelfType(CapabilitySet.Readable, bareSelfType);
        var prefixType = typeConstructor.ParameterTypes[0];
        var readType = bareType.WithDefaultCapability();
        var mutType = bareType.With(Capability.Mutable);
        var itemType = typeConstructor.ParameterTypes[1];
        var irefVarItemType = RefType.Create(new(isInternal: true, isMutableBinding: true, itemType.PlainType), itemType);
        var selfViewIRefVarItemType = new SelfViewpointType(CapabilitySet.Readable, irefVarItemType);
        var classSymbol = new OrdinaryTypeSymbol(@namespace, typeConstructor);
        tree.Add(classSymbol);

        // published init(.fixed, .capacity) {...}
        var initializer = Initializer(classSymbol, mutType, Params(prefixType, Type.Size));
        tree.Add(initializer);

        // TODO should this use `iref` to avoid copying large structs?
        // published get prefix(self) -> P;
        var getPrefix = Getter(classSymbol, "prefix", readType, prefixType);
        tree.Add(getPrefix);

        // published set prefix(mut self, value: P);
        var setPrefix = Setter(classSymbol, "prefix", mutType, Param(prefixType));
        tree.Add(setPrefix);

        // published get capacity(self) -> size;
        var capacity = Getter(classSymbol, "capacity", readType, Type.Size);
        tree.Add(capacity);

        // published get count(self) -> size
        var count = Getter(classSymbol, "count", readType, Type.Size);
        tree.Add(count);

        // published /* unsafe */ fn at(readable self, index: size) -> self |> iref var T
        var at = Method(classSymbol, "at", readableSelfType, Params(Type.Size), selfViewIRefVarItemType);
        tree.Add(at);

        // published fn add(mut self, value: T);
        var add = Method(classSymbol, "add", mutType, Params(itemType));
        tree.Add(add);

        // published fn shrink(mut self, count: size)
        var shrink = Method(classSymbol, "shrink", mutType, Params(Type.Size));
        tree.Add(shrink);

        return typeConstructor;
    }
}
