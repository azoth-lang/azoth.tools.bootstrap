using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FieldDeclarationSyntax : MemberDeclarationSyntax, IFieldDeclarationSyntax
{
    public new IClassOrStructDeclarationSyntax DeclaringType { get; }
    public bool IsMutableBinding { get; }
    public new SimpleName Name { get; }
    public new AcyclicPromise<FieldSymbol> Symbol { [DebuggerStepThrough] get; }
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    public ITypeSyntax Type { get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }

    public FieldDeclarationSyntax(
        IClassOrStructDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        bool mutableBinding,
        TextSpan nameSpan,
        SimpleName name,
        ITypeSyntax type,
        IExpressionSyntax? initializer)
        : base(declaringType, span, file, accessModifier, nameSpan, name, new AcyclicPromise<FieldSymbol>())
    {
        DeclaringType = declaringType;
        IsMutableBinding = mutableBinding;
        Name = name;
        Type = type;
        Initializer = initializer!;
        Symbol = (AcyclicPromise<FieldSymbol>)base.Symbol;
    }

    public override string ToString()
    {
        var result = $"{Name}: {Type}";
        if (Initializer is not null)
            result += Initializer.ToString();
        result += ";";
        return result;
    }
}
