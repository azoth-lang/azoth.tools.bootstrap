using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ILGen
{
    public class ILFactory
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public ControlFlowGraph CreateGraph(IConcreteInvocableDeclaration invocableDeclaration)
        {
            // TODO build control flow graphs for field initializers

            var fabrication = new ControlFlowGraphFabrication(invocableDeclaration);
            return fabrication.CreateGraph();
        }
    }
}
