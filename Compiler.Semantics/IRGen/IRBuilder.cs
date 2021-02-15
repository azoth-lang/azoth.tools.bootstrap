using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IR;
using Azoth.Tools.Bootstrap.Compiler.IR.Declarations;
using Azoth.Tools.Bootstrap.Compiler.IR.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.IRGen
{
    internal class IRBuilder
    {
        private readonly ControlFlowGraphFactory cfgFactory;
        private readonly Dictionary<Symbol, DeclarationIR> declarations = new Dictionary<Symbol, DeclarationIR>();
        private readonly Table<ClassIR> classes = new Table<ClassIR>();
        private readonly Table<FunctionIR> functions = new Table<FunctionIR>();
        private readonly Table<FieldIR> fields = new Table<FieldIR>();
        private readonly Table<ConstructorIR> constructors = new Table<ConstructorIR>();
        private readonly Table<DataType> types = new Table<DataType>();

        public IRBuilder()
        {
            cfgFactory = new ControlFlowGraphFactory(this);
        }

        public FixedList<DeclarationIR> Add(IEnumerable<IDeclaration> declarations)
        {
            return declarations.Select(Add).ToFixedList();
        }

        public DeclarationIR Add(IDeclaration declaration)
        {
            if (declarations.TryGetValue(declaration.Symbol, out var declarationIR))
                return declarationIR;

            switch (declaration)
            {
                default:
                    //throw ExhaustiveMatch.Failed(declaration);
                    throw new NotImplementedException($"IRBuilder for {declaration.GetType().Name}");
                case IFunctionDeclaration function:
                {
                    var cfg = cfgFactory.CreateGraph(function);
                    var functionIR = new FunctionIR(function.Symbol, BuildParameters(function.Parameters), cfg);
                    functions.GetOrAdd(functionIR);
                    declarationIR = functionIR;
                    break;
                }
                //case IAssociatedFunctionDeclaration associatedFunction:
                //{
                //    var il = ilFactory.CreateGraph(associatedFunction);
                //    declarationIL = new FunctionIL(false, true, associatedFunction.Symbol, BuildParameters(associatedFunction.Parameters), il);
                //    break;
                //}
                case IConcreteMethodDeclaration method:
                {
                    var cfg = cfgFactory.CreateGraph(method);
                    var methodIR = new MethodIR(method.Symbol, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), cfg);
                    declarationIR = methodIR;
                    break;
                }
                //case IAbstractMethodDeclaration method:
                //{
                //    declarationIL = new MethodDeclarationIR(method.Symbol, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), null);
                //    break;
                //}
                case IConstructorDeclaration constructor:
                {
                    var cfg = cfgFactory.CreateGraph(constructor);
                    var parameters = BuildParameters(constructor);
                    //var fieldInitializations = BuildFieldInitializations(constructor);
                    var constructorIR = new ConstructorIR(constructor.Symbol, parameters, cfg);
                    constructors.GetOrAdd(constructorIR);
                    declarationIR = constructorIR;
                    break;
                }
                case IFieldDeclaration fieldDeclaration:
                {
                    var fieldIR = new FieldIR(fieldDeclaration.Symbol);
                    fields.GetOrAdd(fieldIR);
                    declarationIR = fieldIR;
                    break;
                }
                case IClassDeclaration classDeclaration:
                {
                    var classIR = new ClassIR(classDeclaration.Symbol, BuildClassMembers(classDeclaration));
                    classes.GetOrAdd(classIR);
                    declarationIR = classIR;
                    break;
                }
            }

            declarations.Add(declarationIR.Symbol, declarationIR);
            return declarationIR;
        }

        private static FixedList<NamedParameterIR> BuildParameters(IEnumerable<INamedParameter> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static NamedParameterIR BuildParameter(INamedParameter parameter)
        {
            return new NamedParameterIR(parameter.Symbol);
        }

        private SelfParameterIR BuildParameter(ISelfParameter selfParameter)
        {
            return new SelfParameterIR(selfParameter.Symbol);
        }

        private static FixedList<ConstructorParameterIR> BuildParameters(IConstructorDeclaration constructorDeclaration)
        {
            return constructorDeclaration.Parameters.Select(BuildParameter).ToFixedList();
        }

        private static ConstructorParameterIR BuildParameter(IConstructorParameter parameter)
        {
            return parameter switch
            {
                INamedParameter namedParameter => BuildParameter(namedParameter),
                IFieldParameter fieldParameter => BuildParameter(fieldParameter),
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }

        private static FieldParameterIR BuildParameter(IFieldParameter fieldParameter)
        {
            return new FieldParameterIR(fieldParameter.ReferencedSymbol);
        }

        private FixedList<MemberIR> BuildClassMembers(IClassDeclaration classDeclaration)
        {
            return Add(classDeclaration.Members).OfType<MemberIR>().ToFixedList();
            //var defaultConstructor = BuildDefaultConstructor(classDeclaration);
            //if (!(defaultConstructor is null)) members = members.Append(defaultConstructor).ToFixedList();
            //return members.ToFixedList();
        }

        private DeclarationIR? BuildDefaultConstructor(IClassDeclaration classDeclaration, ISymbolTree symbolTree)
        {
            var constructorSymbol = classDeclaration.DefaultConstructorSymbol;
            if (constructorSymbol is null) return null;

            if (declarations.TryGetValue(constructorSymbol, out var declaration)) return declaration;

            var selfParameterSymbol = symbolTree.Children(constructorSymbol).OfType<SelfParameterSymbol>().Single();
            var selfParameter = new SelfParameterIR(selfParameterSymbol);
            var parameters = selfParameter.Yield().ToFixedList<ParameterIR>();

            //var graph = new ControlFlowGraphBuilder(classDeclaration.File);
            //graph.AddSelfParameter(selfParameterSymbol);
            //var block = graph.NewBlock();
            //block.End(new ReturnVoidInstruction(classDeclaration.NameSpan, Scope.Outer));

            //var il = new ControlFlowGraphBuilder(classDeclaration.File);
            //il.AddSelfParameter(selfType);
            //var block = il.NewBlock();
            //block.End(classDeclaration.NameSpan, Scope.Outer);

            //var defaultConstructor = new ConstructorIR( // TODO how to get a name
            //    constructorSymbol, parameters, FixedList<FieldInitializationIR>.Empty, graph.Build());

            //defaultConstructor.ControlFlowOld.InsertedDeletes = new InsertedDeletes();
            //declarations.Add(constructorSymbol, defaultConstructor);
            //return defaultConstructor;

            throw new NotImplementedException();
        }

        public void DetermineEntryPoint(Diagnostics diagnostics)
        {
            //var mainFunctions = declarations.OfType<FunctionIL>().Where(f => f.Symbol.Name == "main").ToList();
            //return mainFunctions.SingleOrDefault();

            // TODO warn on and remove main functions that don't have correct parameters or types
            // TODO compiler error on multiple main functions

            //throw new NotImplementedException();
        }

        public PackageIR BuildPackage(Package package)
        {
            // TODO maybe the parts of the package should be taken in the constructor instead of here as the package
            return new PackageIR(
                package.SymbolTree,
                package.Diagnostics.Build(),
                package.ReferencedPackages.ToFixedSet(),
                classes,
                functions,
                types);
        }

        public uint Add(ReferenceType referenceType)
        {
            return types.GetOrAdd(referenceType);
        }
    }
}
