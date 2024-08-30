using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class ContextAspect
{
    public static partial CodeFile CompilationUnit_Children_Broadcast_File(ICompilationUnitNode node)
        => node.File;

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_Children_ContainingDeclaration(INamespaceBlockDefinitionNode node)
        => node.Definition;
}
