using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public static class TypeAttributes
{
    public static Promise<IDeclaredUserType> TypeDeclarationInheritedDeclaredType(ITypeDeclarationNode _)
        => new();

    public static ObjectType Class(IClassDeclarationNode node)
    {
        var packageName = node.Package.Name;
        var typeParameters = GetGenericParameterTypes(node);

        var superTypes = new AcyclicPromise<IFixedSet<BareReferenceType>>();

        // TODO correctly deal with containing namespace
        var containingSymbol = node.ContainingSymbol;
        while (containingSymbol is not NamespaceSymbol)
            containingSymbol = containingSymbol.ContainingSymbol!;
        var containingNamespaceName = ((NamespaceSymbol)containingSymbol).NamespaceName;

        var classType = ObjectType.CreateClass(packageName, containingNamespaceName, node.IsAbstract, node.IsConst,
            node.Name, typeParameters, superTypes);
        return classType;

        // TODO BuildSupertypes(@class, superTypes, typeDeclarations);
    }

    private static IFixedList<GenericParameterType> GetGenericParameterTypes(ITypeDeclarationNode node)
        => node.GenericParameters.Select(p => p.Type).ToFixedList();

    public static GenericParameterType GenericParameter(IGenericParameterNode node)
        => new(node.ContainingDeclaredType,
            new GenericParameter(node.Constraint.Constraint, node.Name, node.Independence, node.Variance));
}
