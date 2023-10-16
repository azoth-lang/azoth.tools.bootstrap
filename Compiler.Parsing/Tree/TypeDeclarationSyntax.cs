using System;
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
    public IAccessModifierToken? AccessModifier { get; }
    public IConstKeywordToken? ConstModifier { get; }
    public bool IsConst { get; }
    public IMoveKeywordToken? MoveModifier { get; }
    public bool IsMove { get; }
    public new StandardTypeName Name { get; }
    public FixedList<IGenericParameterSyntax> GenericParameters { get; }
    public new AcyclicPromise<ObjectTypeSymbol> Symbol { get; }
    public FixedList<ITypeNameSyntax> SuperTypes { get; }
    public abstract FixedList<TMember> Members { get; }
    FixedList<IMemberDeclarationSyntax> ITypeDeclarationSyntax.Members => members.Value;
    private readonly Lazy<FixedList<IMemberDeclarationSyntax>> members;

    public TypeDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        StandardTypeName name,
        FixedList<IGenericParameterSyntax> genericParameters,
        FixedList<ITypeNameSyntax> superTypes)
        : base(containingNamespaceName, headerSpan, file, name, nameSpan, new AcyclicPromise<ObjectTypeSymbol>())
    {
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        IsConst = ConstModifier is not null;
        MoveModifier = moveModifier;
        IsMove = MoveModifier is not null;
        Name = name;
        GenericParameters = genericParameters;
        SuperTypes = superTypes;
        Symbol = (AcyclicPromise<ObjectTypeSymbol>)base.Symbol;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<IMemberDeclarationSyntax>().ToFixedList());
    }
}
