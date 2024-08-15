using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypeDefinitionSyntax<TMember> : NonMemberDefinitionSyntax, ITypeDefinitionSyntax
    where TMember : ITypeMemberDefinitionSyntax
{
    public ITypeDefinitionSyntax? DefiningType { get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public bool IsConst { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public bool IsMove { [DebuggerStepThrough] get; }
    public new StandardName Name { [DebuggerStepThrough] get; }
    TypeName INonMemberEntityDefinitionSyntax.Name => Name;
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public abstract IFixedList<TMember> Members { [DebuggerStepThrough] get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => members.Value;
    private readonly Lazy<IFixedList<ITypeMemberDefinitionSyntax>> members;

    protected TypeDefinitionSyntax(
        NamespaceName containingNamespaceName,
        ITypeDefinitionSyntax? declaringType,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        StandardName name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<IStandardTypeNameSyntax> supertypeNames)
        : base(containingNamespaceName, headerSpan, file, name, nameSpan)
    {
        DefiningType = declaringType;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        IsConst = ConstModifier is not null;
        MoveModifier = moveModifier;
        IsMove = MoveModifier is not null;
        Name = name;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<ITypeMemberDefinitionSyntax>().ToFixedList());
    }
}
