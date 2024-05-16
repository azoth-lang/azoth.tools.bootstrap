using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDefinitionNode : TypeDefinitionNode, IClassDefinitionNode
{
    public override IClassDefinitionSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    public IStandardTypeNameNode? BaseTypeName { get; }

    private ValueAttribute<IClassDeclarationNode> symbolNode;
    public override IClassDeclarationNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.ClassDeclaration_SymbolNode);

    private ValueAttribute<ObjectType> declaredType;
    public override ObjectType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.ClassDeclaration_DeclaredType);

    public override IFixedList<IClassMemberDefinitionNode> Members { get; }
    private ValueAttribute<ConstructorSymbol?> defaultConstructorSymbol;
    public ConstructorSymbol? DefaultConstructorSymbol
        => defaultConstructorSymbol.TryGetValue(out var value) ? value
            : defaultConstructorSymbol.GetValue(this, SymbolAttribute.ClassDeclaration_DefaultConstructorSymbol);

    public ClassDefinitionNode(
        IClassDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IStandardTypeNameNode? baseTypeName,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IClassMemberDefinitionNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        BaseTypeName = Child.Attach(this, baseTypeName);
        Members = ChildList.Attach(this, members);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeDeclarationsAspect.ClassDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
