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

    protected NamespaceSymbol Namespace(string? name = null, NamespaceOrPackageSymbol? ns = null)
        => new(ns ?? Package(), Name(name) ?? DefaultName("namespace"));

    protected PackageSymbol Package(string? name = null)
        => new(Name(name) ?? DefaultName("package"));

    protected SimpleName DefaultName(string prefix) => new($"⧫{prefix}_{++unique}");

    [return: NotNullIfNotNull(nameof(name))]
    protected static SimpleName? Name(string? name = null) => name is null ? null : new SimpleName(name);

    protected IFixedList<Parameter> Params(int? count = null)
        => Enumerable.Range(1, count ?? ++unique).Select(_ => Compiler.Types.Parameters.Parameter.Int).ToFixedList();

    protected static IFixedList<Parameter> Params(DataType param, params DataType[] @params)
        => @params.Prepend(param).Select(t => new Parameter(false, t)).ToFixedList();

    protected static SelfParameter SelfParam(Pseudotype param) => new SelfParameter(false, param);

    protected static Parameter Param(DataType param) => new Parameter(false, param);

    protected FunctionSymbol Func(
        string? name = null,
        NamespaceOrPackageSymbol? ns = null,
        IFixedList<Parameter>? @params = null,
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
        NamespaceOrPackageSymbol? ns = null,
        IFixedList<Parameter>? @params = null,
        Return? @return = null)
    {
        return new FunctionSymbol(
            ns ?? mother.ContainingSymbol,
            Name(name) ?? mother.Name,
            new FunctionType(@params ?? mother.Parameters, @return ?? mother.Return));
    }

    protected MethodSymbol Method(
        string? name = null,
        ObjectTypeSymbol? containing = null,
        SelfParameter? self = null,
        IFixedList<Parameter>? @params = null,
        Return? @return = null)
    {
        containing ??= Type();
        return new MethodSymbol(
            containing,
            Name(name) ?? DefaultName("method"),
            self ?? new SelfParameter(false, containing.DeclaresType.With(ReferenceCapability.Read, FixedList.Empty<DataType>())),
            @params ?? Params(),
            @return ?? ReturnType());
    }

    protected static MethodSymbol Method(
        MethodSymbol mother,
        string? name = null,
        ObjectTypeSymbol? containing = null,
        SelfParameter? self = null,
        IFixedList<Parameter>? @params = null,
        Return? @return = null)
    {
        return new MethodSymbol(
            containing ?? mother.ContainingSymbol,
            Name(name) ?? mother.Name,
            self ?? mother.SelfParameterType,
            @params ?? mother.Parameters,
            @return ?? mother.Return);
    }

    protected ReferenceType<ObjectType> DataType(
        string? name = null,
        SimpleName? containingPackage = null,
        NamespaceName? containingNamespace = null,
        bool? isConst = null,
        ReferenceCapability? referenceCapability = null)
    {
        var finalName = Name(name) ?? DefaultName("DataType");
        return ReferenceType.CreateClass(
            referenceCapability ?? ReferenceCapability.Constant,
            containingPackage ?? DefaultName("package"),
            containingNamespace ?? NamespaceName.Global,
            isAbstract: false,
            isConst ?? false,
            finalName.Text);
    }

    protected Return ReturnType(
        string? name = null,
        SimpleName? containingPackage = null,
        NamespaceName? containingNamespace = null,
        bool? isConst = null,
        ReferenceCapability? referenceCapability = null)
    {
        return new Return(DataType(name, containingPackage, containingNamespace, isConst, referenceCapability));
    }

    protected ObjectTypeSymbol Type(
        NamespaceOrPackageSymbol? ns = null,
        ObjectType? dataType = null)
    {
        return new ObjectTypeSymbol(
            ns ?? Package(),
            dataType ?? DataType().DeclaredType);
    }

    protected VariableSymbol Parameter(
        string? name = null,
        InvocableSymbol? containing = null,
        int? declaration = null,
        bool? mut = null,
        bool? lent = null,
        DataType? type = null)
    {
        return VariableSymbol.CreateParameter(
            containing ?? Func(),
            Name(name) ?? DefaultName("variable"),
            declaration ?? ++unique,
            mut ?? true,
            lent ?? false,
            type ?? DataType());
    }

    protected VariableSymbol LocalVariable(
        string? name = null,
        InvocableSymbol? containing = null,
        int? declaration = null,
        bool? mut = null,
        DataType? type = null)
    {
        return VariableSymbol.CreateLocal(
            containing ?? Func(),
            mut ?? true,
            Name(name) ?? DefaultName("variable"),
            declaration ?? ++unique,
            type ?? DataType());
    }

    protected static VariableSymbol LocalVariable(
        VariableSymbol mother,
        string? name = null,
        InvocableSymbol? containing = null,
        int? declaration = null,
        bool? mut = null,
        DataType? type = null)
    {
        return VariableSymbol.CreateLocal(
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
