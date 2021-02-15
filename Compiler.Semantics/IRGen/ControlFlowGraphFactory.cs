using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.IR.CFG;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.IL.Instructions;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.IL.Instructions.NullaryOpcode;
using Block = Azoth.Tools.Bootstrap.Compiler.IR.CFG.Block;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.IRGen
{
    internal class ControlFlowGraphFactory
    {
        private readonly IRBuilder irBuilder;

        public ControlFlowGraphFactory(IRBuilder irBuilder)
        {
            this.irBuilder = irBuilder;
        }

        public ControlFlowGraph CreateGraph(IConcreteInvocableDeclaration invocable)
        {
            var (selfType, returnType) = GetSelfAndReturnTypes(invocable);

            var graph = new ControlFlowGraph();
            var entryBlock = graph.AddBlock();
            if (selfType != null) AddParameter(entryBlock, selfType);
            foreach (var parameter in invocable.Parameters)
                AddParameter(entryBlock, parameter);

            // TODO initialize any fields

            var currentBlock = entryBlock;
            foreach (var statement in invocable.Body.Statements)
                currentBlock = Convert(statement, graph, currentBlock);

            // Generate the implicit return statement
            //if (currentBlock != null && !currentBlock.IsTerminated)
            //{
            //    var span = invocable.Span.AtEnd();
            //    //EndScope(span);
            //    currentBlock.End(new ReturnVoidInstruction(span, Scope.Outer));
            //}

            return graph;
        }

        private static (DataType? SelfType, DataType ReturnType) GetSelfAndReturnTypes(
            IConcreteInvocableDeclaration invocable)
        {
            return invocable switch
            {
                IConcreteMethodDeclaration method
                    => (method.SelfParameter.Symbol.DataType, method.Symbol.ReturnDataType.Known()),
                IConstructorDeclaration constructor
                    // the body should `return;` so the return type is "void"
                    => (constructor.ImplicitSelfParameter.Symbol.DataType, DataType.Void),
                IAssociatedFunctionDeclaration associatedFunction
                    => (null, associatedFunction.Symbol.ReturnDataType.Known()),
                IFunctionDeclaration function => (null, function.Symbol.ReturnDataType.Known()),
                _ => throw ExhaustiveMatch.Failed(invocable)
            };
        }

        private void AddParameter(Block block, DataType type)
        {
            var parameterInstruction = type switch
            {
                // TODO special case all the default types
                PointerSizedIntegerType t => new Instruction(t.IsSigned ? ParamOffset : ParamSize),
                EmptyType _ => throw new InvalidOperationException("Parameter with empty type"),
                UnknownType _ => throw new InvalidOperationException("Parameter with unknown type"),
                ReferenceType t => new Instruction(ShortOpcode.Param, irBuilder.Add(t)),
                _ => throw new NotImplementedException($"Type: {type}")//throw ExhaustiveMatch.Failed(type)
            };

            block.Add(parameterInstruction);
        }

        private void AddParameter(Block block, IConstructorParameter parameter)
        {
            switch (parameter)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter);
                case INamedParameter namedParameter:
                    AddParameter(block, namedParameter.Symbol.DataType);
                    break;
                case IFieldParameter fieldParameter:
                    AddParameter(block, fieldParameter.ReferencedSymbol.DataType);
                    break;
            }
        }

        private Block Convert(IBodyStatement statement, ControlFlowGraph graph, Block currentBlock)
        {
            //switch (statement)
            //{
            //    default:
            //        throw new NotImplementedException($"Statement Type: {statement.GetType().Name}");
            //        //throw ExhaustiveMatch.Failed(statement);
            //}
            return currentBlock;
        }
    }
}
