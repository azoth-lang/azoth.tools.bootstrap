using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypeDefinitionSyntax<TMember> : NonMemberDefinitionSyntax, ITypeDefinitionSyntax
    where TMember : ITypeMemberDefinitionSyntax
{
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public new StandardName Name { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public abstract IFixedList<TMember> Members { [DebuggerStepThrough] get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => members.Value;
    private readonly Lazy<IFixedList<ITypeMemberDefinitionSyntax>> members;

    protected TypeDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        StandardName name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<IStandardTypeNameSyntax> supertypeNames)
        : base(span, file, name, nameSpan)
    {
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<ITypeMemberDefinitionSyntax>().ToFixedList());
    }
}
