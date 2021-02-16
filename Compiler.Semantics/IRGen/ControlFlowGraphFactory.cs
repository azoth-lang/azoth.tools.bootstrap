using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.IR.CFG;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.IL.Instructions;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.IL.Instructions.NullaryOpcode;
using static Azoth.Tools.Bootstrap.IL.Instructions.ShortOpcode;
using static Azoth.Tools.Bootstrap.IL.Instructions.UnaryOpcode;
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
            var graph = new ControlFlowGraph();

            // TODO remove hack for skipping functions other than main
            if (invocable.Symbol.Name != "main") return graph;

            var (selfType, returnType) = GetSelfAndReturnTypes(invocable);


            var entryBlock = graph.AddBlock();
            if (selfType != null) AddParameter(entryBlock, selfType);
            foreach (var parameter in invocable.Parameters)
                AddParameter(entryBlock, parameter);

            // TODO initialize any fields

            Block? currentBlock = entryBlock;
            foreach (var statement in invocable.Body.Statements)
                Build(statement, graph, ref currentBlock);

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
                ReferenceType t => new Instruction(Param, irBuilder.Add(t)),
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

        private static void Build(IBodyStatement statement, ControlFlowGraph graph, ref Block? currentBlock)
        {
            switch (statement)
            {
                default:
                    throw new NotImplementedException($"Statement Type: {statement.GetType().Name}");
                //throw ExhaustiveMatch.Failed(statement);
                case IExpressionStatement expressionStatement:
                {
                    var expression = expressionStatement.Expression;
                    if (!expression.DataType.Assigned().IsKnown)
                        throw new ArgumentException("Expression must have a known type", nameof(statement));

                    Build(expression, ref currentBlock);
                }
                break;
            }
        }

        private static ushort Build(IExpression expression, ref Block? currentBlock)
        {
            ushort index;
            switch (expression)
            {
                default:
                    throw new NotImplementedException($"Convert({expression.GetType().Name})");
                //throw ExhaustiveMatch.Failed(expression);
                case IImplicitNumericConversionExpression exp:
                {
                    if (exp.Expression.DataType.Assigned().Known() is IntegerConstantType constantType)
                        index = BuildIntegerLiteral(constantType.Value, exp.ConvertToType, currentBlock!);
                    else
                        throw new NotImplementedException();
                    //currentBlock!.Add(new ConvertInstruction(resultPlace, ConvertToOperand(exp.Expression),
                    //    (NumericType)exp.Expression.DataType.Assigned().Known(), exp.ConvertToType,
                    //    exp.Span, CurrentScope));
                }
                break;
                case IIntegerLiteralExpression exp:
                    throw new InvalidOperationException(
                        "Integer literals should have an implicit conversion around them");
                case IReturnExpression exp:
                {

                    if (exp.Value is null)
                        index = currentBlock!.Add(new Instruction(ReturnVoid));
                    else
                    {
                        var returnValue = Build(exp.Value, ref currentBlock);
                        index = currentBlock!.Add(new Instruction(Return, returnValue));
                    }

                    // There is no exit from a return block, hence null for exit block
                    currentBlock = null;
                }
                break;
            }

            return index;
        }

        private static ushort BuildIntegerLiteral(
            BigInteger value,
            NumericType type,
            Block currentBlock)
        {
            ushort index;
            switch (type)
            {
                default:
                    throw ExhaustiveMatch.Failed(type);
                case FixedSizeIntegerType t:
                {
                    index = BuildIntegerLiteral(value, t.IsSigned, t.Bits, currentBlock);
                    if (t.Bits > 32)
                    {
                        var opcode = t.Bits switch
                        {
                            64 => ConvertToI64,
                            128 => ConvertToI128,
                            _ => throw new InvalidOperationException($"Can't convert to {t.Bits} bits")
                        };
                        index = currentBlock.Add(new Instruction(opcode, index));
                    }
                }
                break;
                case PointerSizedIntegerType t:
                {
                    index = BuildIntegerLiteral(value, t.IsSigned, 8, currentBlock);
                    var opcode = t.IsSigned ? ConvertToOffset : ConvertToSize;
                    index = currentBlock.Add(new Instruction(opcode, index));
                }
                break;
                case IntegerConstantType _:
                    throw new InvalidOperationException("Integer constant must be emitted as a specific type");
            }

            return index;
        }

        private static ushort BuildIntegerLiteral(BigInteger value, bool isSigned, int minSize, Block currentBlock)
        {
            if (isSigned && value <= 8388607 && value >= -8388607)
            {
                minSize = Math.Max(minSize, value.GetByteCount() * 8);
                var operand = (int)value;
                var opcode = minSize switch
                {
                    8 => ConstI8,
                    16 => ConstI16,
                    32 => ConstI32,
                    _ => throw new InvalidOperationException($"Invalid bit size {minSize}")
                };

                return currentBlock.Add(new Instruction(opcode, operand));
            }

            if (!isSigned && value< 0x00FFFFFF)
            {
                minSize = Math.Max(minSize, value.GetByteCount(isUnsigned: true) * 8);
                var operand = (uint)value;
                var opcode = minSize switch
                {
                    8 => ConstU8,
                    16 => ConstU16,
                    32 => ConstU32,
                    _ => throw new InvalidOperationException($"Invalid bit size {minSize}")
                };
                return currentBlock.Add(new Instruction(opcode, operand));
            }

            throw new NotImplementedException("Integer constant too large for inline");
        }
    }
}
