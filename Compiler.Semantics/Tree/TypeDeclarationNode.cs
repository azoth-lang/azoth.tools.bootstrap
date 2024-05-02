using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeDeclarationNode : DeclarationNode, ITypeDeclaration
{
    public abstract override ITypeDeclarationSyntax Syntax { get; }
    public IFixedList<IGenericParameter> GenericParameters { get; }
    public IFixedList<ISupertypeName> SupertypeNames { get; }
    public abstract IFixedList<ITypeMemberDeclaration> Members { get; }

    protected TypeDeclarationNode(
        IEnumerable<IGenericParameter> genericParameters,
        IEnumerable<ISupertypeName> supertypeNames)
    {
        GenericParameters = ChildList.CreateFixed(genericParameters);
        SupertypeNames = ChildList.CreateFixed(supertypeNames);
    }
}
