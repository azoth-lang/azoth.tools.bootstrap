using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Tests.Unit;

public abstract class SymbolTestFixture
{
    private int unique;

    protected LocalNamespaceSymbol Namespace(string? name = null, NamespaceSymbol? ns = null)
        => new(ns ?? Package(), Name(name) ?? DefaultName("namespace"));

    protected PackageSymbol Package(string? name = null)
        => new(Name(name) ?? DefaultName("package"));

    protected IdentifierName DefaultName(string prefix) => new($"⧫{prefix}_{++unique}");

    [return: NotNullIfNotNull(nameof(name))]
    protected static IdentifierName? Name(string? name = null) => name is null ? null : new IdentifierName(name);

    protected IFixedList<ParameterType> Params(int? count = null)
        => Enumerable.Range(1, count ?? ++unique).Select(_ => Compiler.Types.Parameters.ParameterType.Int).ToFixedList();

    protected static IFixedList<ParameterType> Params(DataType param, params DataType[] @params)
        => @params.Prepend(param).Select(t => new ParameterType(false, t)).ToFixedList();

    protected static SelfParameterType SelfParam(Pseudotype param) => new SelfParameterType(false, param);

    protected static ParameterType Param(DataType param) => new ParameterType(false, param);

    protected FunctionSymbol Func(
        string? name = null,
        NamespaceSymbol? ns = null,
        IFixedList<ParameterType>? @params = null,
        Return? @return = null)
    {
        return new FunctionSymbol(
            ns ?? Namespace(),
            Name(name) ?? DefaultName("func"),
            new FunctionType(@params ?? Params(), @return ?? ReturnType()));
    }

    protected static FunctionSymbol Func(
        FunctionSymbol mother,
        string? name = null,
        NamespaceSymbol? ns = null,
        IFixedList<ParameterType>? @params = null,
        Return? @return = null)
    {
        return new FunctionSymbol(
            ns ?? mother.ContainingSymbol,
            Name(name) ?? mother.Name,
            new FunctionType(@params ?? mother.Parameters, @return ?? mother.Return));
    }

    protected MethodSymbol Method(
        string? name = null,
        UserTypeSymbol? containing = null,
        SelfParameterType? self = null,
        IFixedList<ParameterType>? @params = null,
        Return? @return = null)
    {
        containing ??= Type();
        return new MethodSymbol(
            containing,
            Name(name) ?? DefaultName("method"),
            self ?? new SelfParameterType(false, containing.DeclaresType.With(Capability.Read, FixedList.Empty<DataType>())),
            @params ?? Params(),
            @return ?? ReturnType());
    }

    protected static MethodSymbol Method(
        MethodSymbol mother,
        string? name = null,
        UserTypeSymbol? containing = null,
        SelfParameterType? self = null,
        IFixedList<ParameterType>? @params = null,
        Return? @return = null)
    {
        return new MethodSymbol(
            containing ?? mother.ContainingSymbol,
            Name(name) ?? mother.Name,
            self ?? mother.SelfParameterType,
            @params ?? mother.Parameters,
            @return ?? mother.Return);
    }

    protected CapabilityType<ObjectType> DataType(
        string? name = null,
        IdentifierName? containingPackage = null,
        NamespaceName? containingNamespace = null,
        bool? isConst = null,
        Capability? referenceCapability = null)
    {
        var finalName = Name(name) ?? DefaultName("DataType");
        return CapabilityType.CreateClass(
            referenceCapability ?? Capability.Constant,
            containingPackage ?? DefaultName("package"),
            containingNamespace ?? NamespaceName.Global,
            isAbstract: false,
            isConst ?? false,
            finalName.Text);
    }

    protected Return ReturnType(
        string? name = null,
        IdentifierName? containingPackage = null,
        NamespaceName? containingNamespace = null,
        bool? isConst = null,
        Capability? referenceCapability = null)
    {
        return new Return(DataType(name, containingPackage, containingNamespace, isConst, referenceCapability));
    }

    protected UserTypeSymbol Type(
        NamespaceSymbol? ns = null,
        ObjectType? dataType = null)
    {
        return new UserTypeSymbol(
            ns ?? Package(),
            dataType ?? DataType().DeclaredType);
    }

    protected NamedVariableSymbol Parameter(
        string? name = null,
        InvocableSymbol? containing = null,
        int? declaration = null,
        bool? mut = null,
        bool? lent = null,
        DataType? type = null)
    {
        return NamedVariableSymbol.CreateParameter(
            containing ?? Func(),
            Name(name) ?? DefaultName("variable"),
            declaration ?? ++unique,
            mut ?? true,
            lent ?? false,
            type ?? DataType());
    }

    protected NamedVariableSymbol LocalVariable(
        string? name = null,
        InvocableSymbol? containing = null,
        int? declaration = null,
        bool? mut = null,
        DataType? type = null)
    {
        return NamedVariableSymbol.CreateLocal(
            containing ?? Func(),
            mut ?? true,
            Name(name) ?? DefaultName("variable"),
            declaration ?? ++unique,
            type ?? DataType());
    }

    protected static NamedVariableSymbol LocalVariable(
        NamedVariableSymbol mother,
        string? name = null,
        InvocableSymbol? containing = null,
        int? declaration = null,
        bool? mut = null,
        DataType? type = null)
    {
        return NamedVariableSymbol.CreateLocal(
            containing ?? mother.ContainingSymbol,
            mut ?? mother.IsMutableBinding,
            Name(name) ?? mother.Name,
            declaration ?? mother.DeclarationNumber,
            type ?? mother.Type);
    }

    protected SelfParameterSymbol SelfParam(
        InvocableSymbol? containing = null,
        bool? lent = null,
        DataType? type = null)
    {
        return new SelfParameterSymbol(
            containing ?? Method(),
            lent ?? false,
            type ?? DataType());
    }
}
