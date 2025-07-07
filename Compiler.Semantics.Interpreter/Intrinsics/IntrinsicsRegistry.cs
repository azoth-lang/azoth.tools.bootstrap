using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using static Azoth.Tools.Bootstrap.Compiler.Symbols.SymbolBuilder;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

/// <summary>
/// This serves as the source of truth for what intrinsics the interpreter supports. All intrinsics
/// implementations must be registered in the singleton <see cref="IntrinsicsRegistry"/> instance.
/// </summary>
// TODO this is built around symbol matching. However, it should be agnostic to what package the intrinsics are in and based on attributes
internal sealed class IntrinsicsRegistry
{
    #region Singleton
    public static IntrinsicsRegistry Instance = Create();

    private IntrinsicsRegistry(
        FrozenDictionary<FunctionSymbol, IntrinsicFunction> functions,
        FrozenDictionary<InitializerSymbol, IntrinsicInitializer> initializers,
        FrozenDictionary<MethodSymbol, IntrinsicMethod> methods)
    {
        this.functions = functions;
        this.initializers = initializers;
        this.methods = methods;
    }
    #endregion

    #region Instance Members
    private readonly FrozenDictionary<FunctionSymbol, IntrinsicFunction> functions;
    private readonly FrozenDictionary<InitializerSymbol, IntrinsicInitializer> initializers;
    private readonly FrozenDictionary<MethodSymbol, IntrinsicMethod> methods;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IntrinsicFunction? Get(FunctionSymbol functionSymbol)
        => functions.GetValueOrDefault(functionSymbol);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IntrinsicInitializer? Get(InitializerSymbol initializerSymbol)
        => initializers.GetValueOrDefault(initializerSymbol);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IntrinsicMethod? Get(MethodSymbol methodSymbol)
        => methods.GetValueOrDefault(methodSymbol);
    #endregion

    private static IntrinsicsRegistry Create()
    {
        var builder = new Builder();

        var intrinsicsPackage = new PackageSymbol("azoth.compiler.intrinsics");
        var intrinsicsFacet = new PackageFacetSymbol(intrinsicsPackage, FacetKind.Main);
        var azothNamespace = new LocalNamespaceSymbol(intrinsicsFacet, "azoth");

        BuildCompilerIntrinsics(builder, azothNamespace);

        BuildRawCollections(builder, azothNamespace);

        return builder.Build();
    }

    private static void BuildCompilerIntrinsics(Builder builder, LocalNamespaceSymbol azothNamespace)
    {
        var compilerNamespace = new LocalNamespaceSymbol(azothNamespace, "compiler");
        var intrinsicsNamespace = new LocalNamespaceSymbol(compilerNamespace, "intrinsics");

        // published fn INTRINSIC() -> never
        var intrinsicFunction = Function(intrinsicsNamespace, "INTRINSIC", Params(), Type.Never);
        builder.Add(intrinsicFunction, (_, args) => throw new AbortException("INTRINSIC called."));
    }

    private static void BuildRawCollections(Builder builder, LocalNamespaceSymbol azothNamespace)
    {
        var collectionsNamespace = new LocalNamespaceSymbol(azothNamespace, "collections");
        var rawNamespace = new LocalNamespaceSymbol(collectionsNamespace, "raw");

        BuildRawHybridArray(builder, rawNamespace);
    }

    private static void BuildRawHybridArray(Builder builder, LocalNamespaceSymbol rawNamespace)
    {
        // published class Raw_Hybrid_Array[P shareable ind, T ind nonwritable out]
        var typeConstructor = BareTypeConstructor.CreateClass(rawNamespace.Package.Name, rawNamespace.NamespaceName,
            isAbstract: false, isConst: false, "Raw_Hybrid_Array",
            TypeConstructorParameter.ShareableIndependent(CapabilitySet.Aliasable, "P"),
            TypeConstructorParameter.IndependentNonWriteableOut(CapabilitySet.Aliasable, "T"));
        var plainType = typeConstructor.ConstructWithParameterPlainTypes();
        var bareType = typeConstructor.ConstructWithParameterTypes(plainType);
        var bareSelfType = BareSelfType(bareType);
        var readSelfType = bareSelfType.With(Capability.Read);
        var readableSelfType = new CapabilitySetSelfType(CapabilitySet.Readable, bareSelfType);
        var initMutableSelfType = bareSelfType.With(Capability.InitMutable);
        var mutSelfType = bareSelfType.With(Capability.Mutable);
        var prefixType = typeConstructor.ParameterTypes[0];
        var readType = bareType.WithDefaultCapability();
        var mutType = bareType.With(Capability.Mutable);
        var initMutableType = bareType.With(Capability.InitMutable);
        var itemType = typeConstructor.ParameterTypes[1];
        var irefVarItemType
            = RefType.Create(new(isInternal: true, isMutableBinding: true, itemType.PlainType), itemType);
        var selfViewIRefVarItemType = new SelfViewpointType(CapabilitySet.Readable, irefVarItemType);
        var classSymbol = new OrdinaryTypeSymbol(rawNamespace, typeConstructor);

        // published /*unsafe*/ init(mut self, ensure_prefix_zeroed: bool, count: size, ensure_zeroed: bool)
        var initializer = Initializer(classSymbol, initMutableSelfType, Params(Type.Bool, Type.Size, Type.Bool));
        builder.Add(initializer, static (selfBareType, _, arguments) =>
        {
            var itemType = selfBareType.Arguments[1];
            var ensurePrefixZeroed = arguments[0].BoolValue;
            var count = arguments[1].NUIntValue;
            var ensureZeroed = arguments[2].BoolValue;
            return ValueTask.FromResult(AzothValue.Intrinsic(RawHybridArray.Create(itemType, ensurePrefixZeroed, count, ensureZeroed)));
        });

        // published unsafe get prefix(self) -> P
        var getPrefix = Getter(classSymbol, "prefix", readSelfType, prefixType);
        builder.Add(getPrefix, static (_, s, args) =>
        {
            var self = s.IntrinsicValue.UnsafeAs<RawHybridArray>();
            return ValueTask.FromResult(self.Prefix);
        });

        // published set prefix(mut self, prefix: P)
        var setPrefix = Setter(classSymbol, "prefix", mutSelfType, Param(prefixType));
        builder.Add(setPrefix, static (_, s, args) =>
        {
            var self = s.IntrinsicValue.UnsafeAs<RawHybridArray>();
            var prefix = args[0];
            self.Prefix = prefix;
            return ValueTask.FromResult(AzothValue.None);
        });

        // published unsafe get count(self) -> size
        var getCount = Getter(classSymbol, "count", readSelfType, Type.Size);
        builder.Add(getCount, static (_, s, args) =>
        {
            var self = s.IntrinsicValue.UnsafeAs<RawHybridArray>();
            return ValueTask.FromResult(AzothValue.Size(self.Count));
        });
    }

    private sealed class Builder
    {
        private readonly Dictionary<FunctionSymbol, IntrinsicFunction> functions = [];
        private readonly Dictionary<InitializerSymbol, IntrinsicInitializer> initializers = [];
        private readonly Dictionary<MethodSymbol, IntrinsicMethod> methods = [];

        public void Add(FunctionSymbol symbol, IntrinsicFunction function)
            => functions.Add(symbol, function);

        public void Add(InitializerSymbol symbol, IntrinsicInitializer function)
            => initializers.Add(symbol, function);

        public void Add(MethodSymbol symbol, IntrinsicMethod function)
            => methods.Add(symbol, function);

        public IntrinsicsRegistry Build()
            => new(functions.ToFrozenDictionary(), initializers.ToFrozenDictionary(), methods.ToFrozenDictionary());
    }
}
