using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DeclarationNumbers;

public class DeclarationNumberAssigner : SyntaxWalker
{
    private readonly Dictionary<IdentifierName, Promise<int?>> lastDeclaration = new();
    private DeclarationNumberAssigner() { }

    public static void AssignIn(IEnumerable<IEntityDefinitionSyntax> entities)
    {
        foreach (var entity in entities)
        {
            var assigner = new DeclarationNumberAssigner();
            assigner.WalkNonNull(entity);
            assigner.AssignSingleDeclarationsNull();
        }
    }

    protected override void WalkNonNull(IConcreteSyntax syntax)
    {
        switch (syntax)
        {
            case IClassDefinitionSyntax _:
                // Skip, will see members separately
                return;
            case INamedParameterSyntax syn:
                ProcessDeclaration(syn.Name, syn.DeclarationNumber);
                break;
            case IVariableDeclarationStatementSyntax syn:
                ProcessDeclaration(syn.Name, syn.DeclarationNumber);
                break;
            case IForeachExpressionSyntax syn:
                ProcessDeclaration(syn.VariableName, syn.DeclarationNumber);
                break;
            case IBindingPatternSyntax syn:
                ProcessDeclaration(syn.Name, syn.DeclarationNumber);
                break;
            case ITypeSyntax _:
                // can't contain declarations
                return;
        }
        WalkChildren(syntax);
    }

    private void ProcessDeclaration(IdentifierName name, Promise<int?> declarationNumber)
    {
        if (lastDeclaration.TryGetValue(name, out var previousDeclarationNumber))
        {
            if (!previousDeclarationNumber.IsFulfilled)
                // There is at least two declarations, start counting from 1
                previousDeclarationNumber.Fulfill(1);
            declarationNumber.Fulfill(previousDeclarationNumber.Result + 1);
            lastDeclaration[name] = declarationNumber;
        }
        else
            lastDeclaration.Add(name, declarationNumber);
    }

    private void AssignSingleDeclarationsNull()
    {
        var unfulfilledDeclarationNumbers = lastDeclaration.Values
                                                           .Where(declarationNumber => !declarationNumber.IsFulfilled);
        foreach (var declarationNumber in unfulfilledDeclarationNumbers)
            // Only a single declaration, don't apply unique numbers
            declarationNumber.Fulfill(null);
    }
}
