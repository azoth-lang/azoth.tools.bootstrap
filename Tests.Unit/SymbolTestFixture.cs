using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Tests.Unit;

public abstract class SymbolTestFixture
{
    private int unique;

    protected LocalNamespaceSymbol Namespace(string? name = null, NamespaceSymbol? ns = null)
        => new(ns ?? Facet(), Name(name) ?? DefaultName("namespace"));

    protected PackageSymbol Package(string? name = null)
        => new(Name(name) ?? DefaultName("package"));

    protected PackageFacetSymbol Facet(PackageSymbol? package = null, FacetKind kind = FacetKind.Main)
        => new(package ?? Package(), kind);

    protected IdentifierName DefaultName(string prefix) => new($"⧫{prefix}_{++unique}");

    [return: NotNullIfNotNull(nameof(name))]
    protected static IdentifierName? Name(string? name = null) => name is null ? null : new IdentifierName(name);

    protected IFixedList<ParameterType> Params(int? count = null)
        => Enumerable.Range(1, count ?? ++unique).Select(_ => IntParameter).ToFixedList();

    protected static IFixedList<ParameterType> Params(NonVoidType param, params NonVoidType[] @params)
        => @params.Prepend(param).Select(t => new ParameterType(false, t)).ToFixedList();

    protected static ParameterType Param(NonVoidType param) => new(false, param);

    protected FunctionSymbol Func(
        string? name = null,
        NamespaceSymbol? ns = null,
        IFixedList<ParameterType>? @params = null,
        Type? @return = null)
    {
        return new FunctionSymbol(
            ns ?? Namespace(),
            Name(name) ?? DefaultName("func"),
            new FunctionType(@params ?? Params(), @return ?? DataType()));
    }

    protected static FunctionSymbol Func(
        FunctionSymbol mother,
        string? name = null,
        NamespaceSymbol? ns = null,
        IFixedList<ParameterType>? @params = null,
        Type? @return = null)
    {
        return new FunctionSymbol(
            ns ?? mother.ContainingSymbol,
            Name(name) ?? mother.Name,
            new FunctionType(@params ?? mother.ParameterTypes, @return ?? mother.ReturnType));
    }

    protected MethodSymbol Method(
        string? name = null,
        OrdinaryTypeSymbol? containing = null,
        NonVoidType? self = null,
        IFixedList<ParameterType>? @params = null,
        Type? @return = null)
    {
        containing ??= Type();
        return new MethodSymbol(
            containing,
            MethodKind.Standard,
            Name(name) ?? DefaultName("method"),
            self ?? containing.TypeConstructor.ConstructNullaryType(containingType: null).WithDefaultCapability(),
            @params ?? Params(),
            @return ?? DataType());
    }

    protected static MethodSymbol Method(
        MethodSymbol mother,
        string? name = null,
        OrdinaryTypeSymbol? containing = null,
        NonVoidType? self = null,
        IFixedList<ParameterType>? @params = null,
        Type? @return = null)
    {
        return new MethodSymbol(
            containing ?? mother.ContainingSymbol,
            MethodKind.Standard,
            Name(name) ?? mother.Name,
            self ?? mother.SelfParameterType,
            @params ?? mother.ParameterTypes,
            @return ?? mother.ReturnType);
    }

    protected CapabilityType DataType(
        string? name = null,
        IdentifierName? containingPackage = null,
        NamespaceName? containingNamespace = null,
        bool? isConst = null,
        Capability? referenceCapability = null)
    {
        var finalName = Name(name) ?? DefaultName("DataType");
        return BareTypeConstructor.CreateClass(
            containingPackage ?? DefaultName("package"),
            containingNamespace ?? NamespaceName.Global,
            isAbstract: false,
            isConst ?? false,
            finalName.Text)
                              .ConstructNullaryType(containingType: null)
                              .With(referenceCapability ?? Capability.Constant);
    }

    protected OrdinaryTypeSymbol Type(
        NamespaceSymbol? ns = null,
        OrdinaryTypeConstructor? dataType = null)
    {
        return new(
            ns ?? Facet(),
            dataType ?? (OrdinaryTypeConstructor)DataType().PlainType.TypeConstructor!);
    }

    public static readonly ParameterType IntParameter = new(false, Compiler.Types.Decorated.Type.Int);
}
