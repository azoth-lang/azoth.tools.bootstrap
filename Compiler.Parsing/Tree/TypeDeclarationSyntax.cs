using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypeDeclarationSyntax<TMember> : NonMemberDeclarationSyntax, ITypeDeclarationSyntax
    where TMember : IMemberDeclarationSyntax
{
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public bool IsConst { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public bool IsMove { [DebuggerStepThrough] get; }
    public new StandardName Name { [DebuggerStepThrough] get; }
    TypeName INonMemberEntityDeclarationSyntax.Name => Name;
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public new AcyclicPromise<UserTypeSymbol> Symbol { [DebuggerStepThrough] get; }
    public IFixedList<ISupertypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public abstract IFixedList<TMember> Members { [DebuggerStepThrough] get; }
    IFixedList<IMemberDeclarationSyntax> ITypeDeclarationSyntax.Members => members.Value;
    private readonly Lazy<IFixedList<IMemberDeclarationSyntax>> members;

    protected TypeDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        StandardName name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<ISupertypeNameSyntax> supertypeNames)
        : base(containingNamespaceName, headerSpan, file, name, nameSpan, new AcyclicPromise<UserTypeSymbol>())
    {
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        IsConst = ConstModifier is not null;
        MoveModifier = moveModifier;
        IsMove = MoveModifier is not null;
        Name = name;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Symbol = (AcyclicPromise<UserTypeSymbol>)base.Symbol;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<IMemberDeclarationSyntax>().ToFixedList());
    }
}
