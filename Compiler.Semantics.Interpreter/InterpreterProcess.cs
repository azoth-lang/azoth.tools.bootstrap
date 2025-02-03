using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;
using Azoth.Tools.Bootstrap.Compiler.Semantics.InterpreterHelpers;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public sealed class InterpreterProcess
{
    public static InterpreterProcess StartEntryPoint(IPackageNode package, IEnumerable<IPackageNode> referencedPackages)
    {
        if (package.EntryPoint is null)
            throw new ArgumentException("Cannot execute package without an entry point");

        return new InterpreterProcess(package, referencedPackages, runTests: false);
    }

    public static InterpreterProcess StartTests(IPackageNode package, IEnumerable<IPackageNode> referencedPackages)
        => new(package, referencedPackages, runTests: true);

    public TimeSpan RunTime => runStopwatch.Elapsed;

    private readonly IPackageNode package;
    private readonly Task executionTask;
    private readonly FrozenDictionary<FunctionSymbol, IConcreteFunctionInvocableDefinitionNode> functions;
    private readonly FrozenDictionary<MethodSymbol, IMethodDefinitionNode> structMethods;
    private readonly FrozenDictionary<InitializerSymbol, IOrdinaryInitializerDefinitionNode?> initializers;
    private readonly FrozenDictionary<OrdinaryTypeSymbol, ITypeDefinitionNode> userTypes;
    private readonly IStructDefinitionNode stringStruct;
    private readonly IOrdinaryInitializerDefinitionNode stringInitializer;
    private readonly IStructDefinitionNode? rangeStruct;
    private readonly InitializerSymbol? rangeInitializer;
    private byte? exitCode;
    private readonly TextWriter standardOutputWriter;
    private readonly MethodSignatureCache methodSignatures = new();
    private readonly ConcurrentDictionary<IClassDefinitionNode, VTable> vTables
        = new(ReferenceEqualityComparer.Instance);
    private readonly ConcurrentDictionary<IStructDefinitionNode, StructLayout> structLayouts
        = new(ReferenceEqualityComparer.Instance);
    private readonly LocalVariables.Scope.Pool localVariableScopePool = new();
    private readonly Stopwatch runStopwatch = new();

    private InterpreterProcess(IPackageNode package, IEnumerable<IPackageNode> referencedPackages, bool runTests)
    {
        this.package = package;
        var allDefinitions = GetAllDefinitions(package, referencedPackages,
            runTests ? r => r.MainFacet.Definitions.Concat(r.TestingFacet.Definitions) : r => r.MainFacet.Definitions);
        functions = allDefinitions
                    .OfType<IConcreteFunctionInvocableDefinitionNode>()
                    .ToFrozenDictionary(f => f.Symbol.Assigned());

        structMethods = allDefinitions
                        .OfType<IMethodDefinitionNode>()
                        .Where(m => m.Symbol.Assigned().ContextTypeSymbol is OrdinaryTypeSymbol { Kind: TypeKind.Struct })
                        .ToFrozenDictionary(m => m.Symbol.Assigned());

        userTypes = allDefinitions.OfType<ITypeDefinitionNode>()
                                 .ToFrozenDictionary(c => c.Symbol);
        stringStruct = userTypes.Values.OfType<IStructDefinitionNode>().Single(c => c.Symbol.Name == SpecialNames.StringTypeName);
        stringInitializer = stringStruct.Members.OfType<IOrdinaryInitializerDefinitionNode>().Single(c => c.Parameters.Count == 3);
        rangeStruct = userTypes.Values.OfType<IStructDefinitionNode>().SingleOrDefault(c => c.Symbol.Name == SpecialNames.RangeTypeName);
        rangeInitializer = rangeStruct?.Members.OfType<IInitializerDefinitionNode>().SingleOrDefault(c => c.Parameters.Count == 2)?.Symbol;

        var defaultInitializerSymbols = allDefinitions
                                       .OfType<IStructDefinitionNode>()
                                       .Select(c => c.DefaultInitializer?.Symbol)
                                       .Concat(allDefinitions
                                               .OfType<IClassDefinitionNode>()
                                               .Select(c => c.DefaultInitializer?.Symbol))
                                       .WhereNotNull();
        initializers = defaultInitializerSymbols
                       .Select(c => (c, default(IOrdinaryInitializerDefinitionNode)))
                       .Concat(allDefinitions
                               .OfType<IOrdinaryInitializerDefinitionNode>()
                               .Select(c => (c.Symbol.Assigned(), (IOrdinaryInitializerDefinitionNode?)c)))
                       .ToFrozenDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

        var pipe = new Pipe();
        standardOutputWriter = new StreamWriter(pipe.Writer.AsStream(), Encoding.UTF8);
        StandardOutput = new StreamReader(pipe.Reader.AsStream(), Encoding.UTF8);

        executionTask = runTests ? Task.Run(RunTestsAsync) : Task.Run(CallEntryPointAsync);
    }

    private static IFixedList<IDefinitionNode> GetAllDefinitions(
        IPackageNode package,
        IEnumerable<IPackageNode> referencedPackages,
        Func<IPackageNode, IEnumerable<IFacetMemberDefinitionNode>> getPackageMemberDefinitions)
        => GetAllDefinitions(referencedPackages.Prepend(package).SelectMany(getPackageMemberDefinitions)).ToFixedList();

    private static IEnumerable<IDefinitionNode> GetAllDefinitions(
        IEnumerable<IFacetMemberDefinitionNode> packageMemberDefinitions)
    {
        var definitions = new Queue<IDefinitionNode>();
        definitions.EnqueueRange(packageMemberDefinitions);
        while (definitions.TryDequeue(out var definition))
        {
            yield return definition;
            if (definition is ITypeDefinitionNode syn)
                definitions.EnqueueRange(syn.Members);
        }
    }

    private async Task CallEntryPointAsync()
    {
        runStopwatch.Start();
        await using var _ = standardOutputWriter;

        var entryPoint = package.EntryPoint!;
        var parameterTypes = entryPoint.Symbol.Assigned().ParameterTypes;
        var arguments = new List<AzothValue>(parameterTypes.Count);
        foreach (var parameterType in parameterTypes)
            arguments.Add(await InitializeMainParameterAsync(parameterType.Type));

        var returnValue = await CallFunctionAsync(entryPoint, arguments).ConfigureAwait(false);
        runStopwatch.Stop();

        // Flush any buffered output
        await standardOutputWriter.FlushAsync().ConfigureAwait(false);
        var returnType = entryPoint.Symbol.Assigned().ReturnType;
        if (returnType.Equals(Type.Void))
            exitCode = 0;
        else if (returnType.Equals(Type.Byte))
            exitCode = returnValue.ByteValue;
        else
            throw new InvalidOperationException($"Main function cannot have return type {returnType.ToILString()}");
    }

    private async Task RunTestsAsync()
    {
        runStopwatch.Start();
        await using var _ = standardOutputWriter;

        var testFunctions = package.TestingFacet.Definitions.OfType<IFunctionDefinitionNode>()
                                   .Where(f => f.Attributes.Any(IsTestAttribute)).ToFixedSet();

        await standardOutputWriter.WriteLineAsync($"Testing {package.Symbol.Name} package...");
        await standardOutputWriter.WriteLineAsync($"  Found {testFunctions.Count} tests");
        await standardOutputWriter.WriteLineAsync();

        int failed = 0;

        var stopwatch = new Stopwatch();
        foreach (var function in testFunctions)
        {
            // TODO check that return type is void
            var symbol = function.Symbol;
            await standardOutputWriter.WriteLineAsync($"{symbol.Assigned().ContainingSymbol.ToILString()}.{symbol.Assigned().Name} ...");
            try
            {
                stopwatch.Start();
                await CallFunctionAsync(function, []).ConfigureAwait(false);
                stopwatch.Stop();
                await standardOutputWriter.WriteLineAsync($"  passed in {stopwatch.Elapsed.ToTotalSecondsAndMilliseconds()}");
            }
            catch (Abort ex)
            {
                await standardOutputWriter.WriteLineAsync("  FAILED: " + ex.Message);
                failed += 1;
            }
        }

        await standardOutputWriter.WriteLineAsync();
        await standardOutputWriter.WriteLineAsync($"Tested {package.Symbol.Name} package");
        await standardOutputWriter.WriteLineAsync($"{failed} failed tests out of {testFunctions.Count} total");

        // Flush any buffered output
        await standardOutputWriter.FlushAsync().ConfigureAwait(false);

        runStopwatch.Stop();
    }

    private static bool IsTestAttribute(IAttributeNode attribute)
        => attribute.TypeName.ReferencedDeclaration!.Name.Text == "Test_Attribute";

    private ValueTask<AzothValue> InitializeMainParameterAsync(Type parameterType)
    {
        if (parameterType is not CapabilityType { Arguments.Count: 0 } type)
            throw new InvalidOperationException(
                $"Parameter to main of type {parameterType.ToILString()} not supported");

        // TODO further restrict what can be passed to main
        // TODO support passing structs to main?
        var @class = userTypes.Values.OfType<IClassDefinitionNode>().Single(c => c.Symbol.TypeConstructor.Equals(type.TypeConstructor));
        var initializerSymbol = NoArgInitializerSymbol(@class);
        return CallInitializerAsync(initializerSymbol, []);
    }

    internal ValueTask<AzothValue> CallFunctionAsync(FunctionSymbol functionSymbol, IReadOnlyList<AzothValue> arguments)
    {
        if (ReferenceEquals(functionSymbol.Package, Intrinsic.SymbolTree.Package))
            return CallIntrinsicAsync(functionSymbol, arguments);
        return CallFunctionAsync(functions[functionSymbol], arguments);
    }

    private async ValueTask<AzothValue> CallFunctionAsync(
        IConcreteFunctionInvocableDefinitionNode function,
        IReadOnlyList<AzothValue> arguments)
    {
        using var scope = localVariableScopePool.CreateRoot();
        var parameters = function.Parameters;
        // For loop avoid Linq Zip
        for (int i = 0; i < parameters.Count; i++)
            scope.Add(parameters[i], arguments[i]);

        var result = await ExecuteAsync(function.Body.Statements, scope).ConfigureAwait(false);
        return result.ReturnValue;
    }

    internal ValueTask<AzothValue> CallInitializerAsync(InitializerSymbol initializerSymbol, IReadOnlyList<AzothValue> arguments)
    {
        if (ReferenceEquals(initializerSymbol.Package, Intrinsic.SymbolTree.Package))
            return CallIntrinsicAsync(initializerSymbol, arguments);
        var typeDefinition = userTypes[initializerSymbol.ContextTypeSymbol];
        return typeDefinition switch
        {
            IClassDefinitionNode @class => CallClassInitializerAsync(@class, initializerSymbol, arguments),
            IStructDefinitionNode @struct => CallStructInitializerAsync(@struct, initializerSymbol, arguments),
            ITraitDefinitionNode _ => throw new UnreachableException("Traits don't have initializers."),
            _ => throw ExhaustiveMatch.Failed(typeDefinition)
        };
    }

    private ValueTask<AzothValue> CallClassInitializerAsync(
        IClassDefinitionNode @class,
        InitializerSymbol initializerSymbol,
        IReadOnlyList<AzothValue> arguments)
    {
        var vTable = vTables.GetOrAdd(@class, CreateVTable);
        var self = AzothValue.Object(new(vTable));
        return CallInitializerAsync(@class, initializerSymbol, self, arguments);
    }

    private static InitializerSymbol NoArgInitializerSymbol(IClassDefinitionNode baseClass)
        => baseClass.DefaultInitializer?.Symbol
           ?? baseClass.Members.OfType<IInitializerDefinitionNode>()
                       .Select(c => c.Symbol.Assigned()).Single(c => c.Arity == 0);

    private ValueTask<AzothValue> CallStructInitializerAsync(
        IStructDefinitionNode @struct,
        InitializerSymbol initializerSymbol,
        IReadOnlyList<AzothValue> arguments)
    {
        var layout = structLayouts.GetOrAdd(@struct, CreateStructLayout);
        var self = AzothValue.Struct(new(layout));
        return CallInitializerAsync(@struct, initializerSymbol, self, arguments);
    }

    private ValueTask<AzothValue> CallInitializerAsync(
        ITypeDefinitionNode typeDefinition,
        InitializerSymbol initializerSymbol,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        // TODO run field initializers
        var initializer = initializers[initializerSymbol];
        // Default initializer is null
        if (initializer is null) return CallDefaultInitializerAsync(typeDefinition, self);
        return CallInitializerAsync(initializer, self, arguments);
    }

    private async ValueTask<AzothValue> CallInitializerAsync(
        IOrdinaryInitializerDefinitionNode initializer,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        using var scope = localVariableScopePool.CreateRoot();
        scope.Add(initializer.SelfParameter, self);
        var parameters = initializer.Parameters;
        // For loop avoid Linq Zip
        for (int i = 0; i < parameters.Count; i++)
            switch (parameters[i])
            {
                default:
                    throw ExhaustiveMatch.Failed(parameters[i]);
                case IFieldParameterNode fieldParameter:
                    self.InstanceValue[fieldParameter.ReferencedField!] = arguments[i];
                    break;
                case INamedParameterNode p:
                    scope.Add(p, arguments[i]);
                    break;
            }

        foreach (var statement in initializer.Body.Statements)
        {
            var result = await ExecuteAsync(statement, scope).ConfigureAwait(false);
            if (result.IsReturn) break;
            if (result.Type != AzothResultType.Ordinary) throw new UnreachableException("Unmatched break or next.");
        }
        return self;
    }

    /// <summary>
    /// Call the implicit default initializer for a type that has no initializers.
    /// </summary>
    private async ValueTask<AzothValue> CallDefaultInitializerAsync(
        ITypeDefinitionNode typeDefinition, AzothValue self)
    {
        // Initialize fields to default values
        var fields = typeDefinition.Members.OfType<IFieldDefinitionNode>();
        foreach (var field in fields)
            self.InstanceValue[field] = AzothValue.None;

        if (typeDefinition is IClassDefinitionNode
            {
                BaseTypeName.ReferencedDeclaration.Symbol: OrdinaryTypeSymbol
                 baseClassSymbol
            })
        {
            var baseClass = (IClassDefinitionNode)userTypes[baseClassSymbol];
            var noArgConstructorSymbol = NoArgInitializerSymbol(baseClass);
            await CallInitializerAsync(baseClass, noArgConstructorSymbol, self, []);
        }

        return self;
    }

    internal ValueTask<AzothValue> CallMethodAsync(
        MethodSymbol methodSymbol,
        Type selfType,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        if (ReferenceEquals(methodSymbol.Package, Intrinsic.SymbolTree.Package))
            return CallIntrinsicAsync(methodSymbol, self, arguments);
        if (ReferenceEquals(methodSymbol, Primitive.IdentityHash))
            return ValueTask.FromResult(IdentityHash(self));

        switch (selfType)
        {
            case CapabilityType capabilityType:
                return CallMethodAsync(methodSymbol, capabilityType, self, arguments);
            case VoidType _:
            case NeverType _:
            case OptionalType _:
            case GenericParameterType _:
            case FunctionType _:
            case CapabilitySetSelfType _:
            case CapabilityViewpointType _:
            case SelfViewpointType _:
            case RefType _:
                var methodSignature = methodSignatures[methodSymbol];
                throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}");
            default:
                throw ExhaustiveMatch.Failed(selfType);
        }
    }

    private ValueTask<AzothValue> CallMethodAsync(
        MethodSymbol methodSymbol,
        CapabilityType selfType,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        var referenceCall = selfType.TypeConstructor.Semantics switch
        {
            // TODO this is an odd case, generic instantiation should avoid it but this works for now
            null => self.InstanceValue.IsObject,
            TypeSemantics.Value => false,
            TypeSemantics.Reference => true,
            _ => throw ExhaustiveMatch.Failed(selfType.TypeConstructor.Semantics),
        };

        return referenceCall
            ? CallClassMethodAsync(methodSymbol, self, arguments)
            : CallStructMethod(methodSymbol, selfType, self, arguments);
    }

    private ValueTask<AzothValue> CallClassMethodAsync(
        MethodSymbol methodSymbol,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        var methodSignature = methodSignatures[methodSymbol];
        var vtable = self.ObjectValue.VTable;
        var method = vtable[methodSignature];
        return CallMethodAsync(method, self, arguments);
    }

    private ValueTask<AzothValue> CallStructMethod(
        MethodSymbol methodSymbol,
        CapabilityType selfType,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        return methodSymbol.Name.Text switch
        {
            "remainder" => ValueTask.FromResult(Remainder(self, arguments.Single(), selfType)),
            "to_display_string" => ToDisplayStringAsync(self, selfType),
            _ => CallMethodAsync(structMethods[methodSymbol], self, arguments),
        };
    }

    private async ValueTask<AzothValue> CallMethodAsync(
        IMethodDefinitionNode method,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        if (method.Body is null)
            throw new InvalidOperationException($"Can't call abstract method {method.Syntax}");

        using var scope = localVariableScopePool.CreateRoot();
        scope.Add(method.SelfParameter, self);
        var parameters = method.Parameters;
        // For loop avoid Linq Zip
        for (int i = 0; i < parameters.Count; i++)
            scope.Add(parameters[i], arguments[i]);

        var result = await ExecuteAsync(method.Body.Statements, scope).ConfigureAwait(false);
        return result.ReturnValue;
    }

    private async ValueTask<AzothResult> ExecuteAsync(IFixedList<IStatementNode> statements, LocalVariables variables)
    {
        foreach (var statement in statements)
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IBodyStatementNode bodyStatement:
                    var result = await ExecuteAsync(bodyStatement, variables).ConfigureAwait(false);
                    if (result.ShouldExit()) return result;
                    break;
                case IResultStatementNode resultStatement:
                    return await ExecuteAsync(resultStatement.Expression!, variables).ConfigureAwait(false);
            }

        return AzothValue.None;
    }

    private async ValueTask<AzothResult> ExecuteAsync(IBodyStatementNode statement, LocalVariables variables)
    {
        switch (statement)
        {
            default:
                throw ExhaustiveMatch.Failed(statement);
            case IExpressionStatementNode s:
                return await ExecuteAsync(s.Expression!, variables).ConfigureAwait(false);
            case IVariableDeclarationStatementNode d:
            {
                var initialValueResult = d.Initializer is null
                    ? AzothValue.None
                    : await ExecuteAsync(d.Initializer, variables).ConfigureAwait(false);
                if (initialValueResult.ShouldExit(out var initialValue)) return initialValueResult;
                variables.Add(d, initialValue);
                return AzothValue.None;
            }
        }
    }

    private ValueTask<AzothResult> ExecuteAsync(IResultStatementNode statement, LocalVariables variables)
        => ExecuteAsync(statement.Expression!, variables);

    private async ValueTask<AzothResult> ExecuteAsync(IExpressionNode expression, LocalVariables variables)
    {
        // A switch on type compiles to essentially a long if/else change of `is Type t` pattern
        // matches. This code spends most of its time determining the node type. To really make this
        // faster would require some kind of dictionary lookup on type or an enum of node types etc.
        // Another thing that could help is an LRU cache of the proper code to run for a given node
        // since most nodes are run repeatedly. Lacking all that, the switch has been broken into
        // nested switches that can then compress multiple checks into a single check for a base
        // type.

        // Also, more common operations should be earlier in the switch

        switch (expression.ExpressionKind)
        {
            default:
                throw ExhaustiveMatch.Failed(expression.ExpressionKind);

            #region Expressions
            case ExpressionKind.Block:
            {
                var block = (IBlockExpressionNode)expression;
                using var scope = variables.CreateNestedScope();
                return await ExecuteAsync(block.Statements, scope).ConfigureAwait(false);
            }
            case ExpressionKind.Unsafe:
            {
                var exp = (IUnsafeExpressionNode)expression;
                return await ExecuteAsync(exp.Expression!, variables).ConfigureAwait(false);
            }
            #endregion

            #region Instance Member Access Expressions
            case ExpressionKind.FieldAccess:
            {
                var exp = (IFieldAccessExpressionNode)expression;
                var result = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var obj)) return result;
                return obj.InstanceValue[exp.ReferencedDeclaration];
            }
            case ExpressionKind.MethodAccess:
            {
                var exp = (IMethodAccessExpressionNode)expression;
                var selfResult = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var methodSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = exp.Context.Type.Known();
                return AzothValue.FunctionReference(new MethodReference(this, selfType, self, methodSymbol));
            }
            #endregion

            #region Literal Expressions
            case ExpressionKind.BoolLiteral:
            {
                var exp = (IBoolLiteralExpressionNode)expression;
                return AzothValue.Bool(exp.Value);
            }
            case ExpressionKind.IntegerLiteral:
            {
                var exp = (IIntegerLiteralExpressionNode)expression;
                return AzothValue.Int(exp.Value);
            }
            case ExpressionKind.NoneLiteral:
                return AzothValue.None;
            case ExpressionKind.StringLiteral:
            {
                var exp = (IStringLiteralExpressionNode)expression;
                // Call the constructor of the string class
                return await InitializeStringAsync(exp.Value).ConfigureAwait(false);
            }
            #endregion

            #region Operator Expressions
            case ExpressionKind.Assignment:
            {
                var exp = (IAssignmentExpressionNode)expression;
                // TODO this evaluates the left hand side twice for compound operators
                var result = exp.Operator switch
                {
                    AssignmentOperator.Simple =>
                        // TODO the expression being assigned into is supposed to be evaluated first
                        await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Plus
                        => await AddAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Minus
                        => await SubtractAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Asterisk
                        => await MultiplyAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Slash
                        => await DivideAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
                if (result.ShouldExit(out var value)) return result;
                return await ExecuteAssignmentAsync(exp.LeftOperand!, value, variables).ConfigureAwait(false);
            }
            case ExpressionKind.BinaryOperator:
            {
                var exp = (IBinaryOperatorExpressionNode)expression;
                switch (exp.Operator)
                {
                    default:
                        throw ExhaustiveMatch.Failed();
                    case BinaryOperator.Plus:
                        return await AddAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.Minus:
                        return await SubtractAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Asterisk:
                        return await MultiplyAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Slash:
                        return await DivideAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.EqualsEquals:
                        return await BuiltInEqualsAsync(exp.NumericOperatorCommonPlainType!, exp.LeftOperand!,
                            exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.NotEqual:
                        return Not(await BuiltInEqualsAsync(exp.NumericOperatorCommonPlainType!, exp.LeftOperand!,
                            exp.RightOperand!, variables).ConfigureAwait(false));
                    case BinaryOperator.ReferenceEquals:
                        return await ReferenceEqualsAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.NotReferenceEqual:
                        return Not(await ReferenceEqualsAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.LessThan:
                    {
                        var result = await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return AzothValue.Bool(value.I32Value < 0);
                    }
                    case BinaryOperator.LessThanOrEqual:
                    {
                        var result = await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return AzothValue.Bool(value.I32Value <= 0);
                    }
                    case BinaryOperator.GreaterThan:
                    {
                        var result = await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return AzothValue.Bool(value.I32Value > 0);
                    }
                    case BinaryOperator.GreaterThanOrEqual:
                    {
                        var result = await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return AzothValue.Bool(value.I32Value >= 0);
                    }
                    case BinaryOperator.And:
                    {
                        var leftResult = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (!left.BoolValue) return AzothValue.Bool(false);
                        return await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.Or:
                    {
                        var leftResult = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (left.BoolValue) return AzothValue.Bool(true);
                        return await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.DotDot:
                    case BinaryOperator.LessThanDotDot:
                    case BinaryOperator.DotDotLessThan:
                    case BinaryOperator.LessThanDotDotLessThan:
                    {
                        var leftResult = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (!exp.Operator.RangeInclusiveOfStart()) left = left.Increment(Type.Int);
                        var rightResult = await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                        if (rightResult.ShouldExit(out var right)) return rightResult;
                        if (exp.Operator.RangeInclusiveOfEnd()) right = right.Increment(Type.Int);
                        return await CallStructInitializerAsync(rangeStruct!, rangeInitializer!, [left, right]);
                    }
                    case BinaryOperator.QuestionQuestion:
                    {
                        var leftResult = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (!left.IsNone) return left;
                        return await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                }
            }
            case ExpressionKind.UnaryOperator:
            {
                var exp = (IUnaryOperatorExpressionNode)expression;
                return exp.Operator switch
                {
                    UnaryOperator.Not => Not(await ExecuteAsync(exp.Operand!, variables).ConfigureAwait(false)),
                    UnaryOperator.Minus => await NegateAsync(exp.Operand!, variables).ConfigureAwait(false),
                    UnaryOperator.Plus => await ExecuteAsync(exp.Operand!, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
            }
            case ExpressionKind.Conversion:
            {
                var exp = (IConversionExpressionNode)expression;
                var result = await ExecuteAsync(exp.Referent!, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return value.Convert(exp.Referent!.Type.Known(), (CapabilityType)exp.ConvertToType.NamedType, false);
            }
            case ExpressionKind.ImplicitConversion:
            {
                var exp = (IImplicitConversionExpressionNode)expression;
                var result = await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return value.Convert(exp.Referent.Type.Known(), (CapabilityType)exp.Type, true);
            }
            case ExpressionKind.PatternMatch:
            {
                var exp = (IPatternMatchExpressionNode)expression;
                var result = await ExecuteAsync(exp.Referent!, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return await ExecuteMatchAsync(value, exp.Pattern, variables);
            }
            case ExpressionKind.Ref:
            {
                var exp = (IRefExpressionNode)expression;
                throw new NotImplementedException();
            }
            case ExpressionKind.ImplicitDeref:
            {
                var exp = (IImplicitDerefExpressionNode)expression;
                throw new NotImplementedException();
            }
            #endregion

            #region Control Flow Expressions
            case ExpressionKind.If:
            {
                var exp = (IIfExpressionNode)expression;
                var conditionResult = await ExecuteAsync(exp.Condition!, variables).ConfigureAwait(false);
                if (conditionResult.ShouldExit(out var condition)) return conditionResult;
                if (condition.BoolValue)
                    return await ExecuteBlockOrResultAsync(exp.ThenBlock, variables).ConfigureAwait(false);
                if (exp.ElseClause is not null)
                    return await ExecuteElseAsync(exp.ElseClause, variables).ConfigureAwait(false);
                return AzothValue.None;
            }
            case ExpressionKind.Loop:
            {
                var exp = (ILoopExpressionNode)expression;
                while (true)
                {
                    var result = await ExecuteAsync(exp.Block, variables).ConfigureAwait(false);
                    switch (result.Type)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(result.Type);
                        case AzothResultType.Next:
                        case AzothResultType.Ordinary:
                            continue;
                        case AzothResultType.Break:
                            return result.Value;
                        case AzothResultType.Return:
                            return result;
                    }
                }
            }
            case ExpressionKind.While:
            {
                var exp = (IWhileExpressionNode)expression;
                var block = exp.Block; // Lifted out of loop
                while (true)
                {
                    // Create a variable scope in case a variable is declared by a pattern in the condition
                    using var scope = variables.CreateNestedScope();
                    var conditionResult = await ExecuteAsync(exp.Condition!, scope).ConfigureAwait(false);
                    if (conditionResult.ShouldExit(out var condition)) return conditionResult;
                    if (!condition.BoolValue)
                        return AzothValue.None;

                    var result = await ExecuteAsync(block, scope).ConfigureAwait(false);
                    switch (result.Type)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(result.Type);
                        case AzothResultType.Next:
                        case AzothResultType.Ordinary:
                            continue;
                        case AzothResultType.Break:
                            return result.Value;
                        case AzothResultType.Return:
                            return result;
                    }
                }
            }
            case ExpressionKind.Foreach:
            {
                var exp = (IForeachExpressionNode)expression;
                var iterableResult = await ExecuteAsync(exp.InExpression!, variables).ConfigureAwait(false);
                if (iterableResult.ShouldExit(out var iterable)) return iterableResult;
                IBindingNode loopVariable = exp;
                // Call `iterable.iterate()` if it exists
                AzothValue iterator;
                CapabilityType iteratorType;
                if (exp.ReferencedIterateMethod is not null)
                {
                    var selfType = (CapabilityType)exp.InExpression!.Type;
                    var iterateMethod = exp.ReferencedIterateMethod!.Symbol!;
                    iterator = await CallMethodAsync(iterateMethod, selfType, iterable, []).ConfigureAwait(false);
                    iteratorType = (CapabilityType)iterateMethod.ReturnType;
                }
                else
                {
                    iterator = iterable;
                    iteratorType = (CapabilityType)exp.InExpression!.Type;
                }

                var nextMethod = exp.ReferencedNextMethod!.Symbol!;
                var block = exp.Block; // Lifted out of loop
                while (true)
                {
                    var value = await CallMethodAsync(nextMethod, iteratorType, iterator, []).ConfigureAwait(false);
                    if (value.IsNone) break;

                    using var scope = variables.CreateNestedScope();
                    scope.Add(loopVariable, value);
                    var result = await ExecuteAsync(block, scope).ConfigureAwait(false);
                    switch (result.Type)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(result.Type);
                        case AzothResultType.Next:
                        case AzothResultType.Ordinary:
                            continue;
                        case AzothResultType.Break:
                            return result.Value;
                        case AzothResultType.Return:
                            return result;
                    }
                }

                return AzothValue.None;
            }
            case ExpressionKind.Break:
            {
                var exp = (IBreakExpressionNode)expression;
                if (exp.Value is null) return AzothResult.BreakWithoutValue;
                var result = await ExecuteAsync(exp.Value, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return AzothResult.Break(value);
            }
            case ExpressionKind.Next:
                return AzothResult.Next;
            case ExpressionKind.Return:
            {
                var exp = (IReturnExpressionNode)expression;
                var value = exp.Value;
                if (value is null) return AzothResult.ReturnVoid;
                var result = await ExecuteAsync(value, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var returnValue)) return result;
                return AzothResult.Return(returnValue);
            }
            #endregion

            #region Invocation Expressions
            case ExpressionKind.FunctionInvocation:
            {
                var exp = (IFunctionInvocationExpressionNode)expression;
                var argumentsResult = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                var functionSymbol = exp.Function.ReferencedDeclaration!.Symbol.Assigned();
                return await CallFunctionAsync(functionSymbol, arguments.ArgumentsValue).ConfigureAwait(false);
            }
            case ExpressionKind.MethodInvocation:
            {
                var exp = (IMethodInvocationExpressionNode)expression;
                var selfResult = await ExecuteAsync(exp.Method.Context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var argumentsResult = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                var methodSymbol = exp.Method.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = exp.Method.Context.Type.Known();
                return await CallMethodAsync(methodSymbol, selfType, self, arguments.ArgumentsValue).ConfigureAwait(false);
            }
            case ExpressionKind.GetterInvocation:
            {
                var exp = (IGetterInvocationExpressionNode)expression;
                var selfResult = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var getterSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = exp.Context.Type.Known();
                return await CallMethodAsync(getterSymbol, selfType, self, []).ConfigureAwait(false);
            }
            case ExpressionKind.SetterInvocation:
            {
                var exp = (ISetterInvocationExpressionNode)expression;
                var selfResult = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var valueResult = await ExecuteAsync(exp.Value!, variables).ConfigureAwait(false);
                if (valueResult.ShouldExit(out var value)) return valueResult;
                var setterSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = exp.Context.Type.Known();
                return await CallMethodAsync(setterSymbol, selfType, self, [value]).ConfigureAwait(false);
            }
            case ExpressionKind.FunctionReferenceInvocation:
            {
                var exp = (IFunctionReferenceInvocationExpressionNode)expression;
                var functionResult = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                if (functionResult.ShouldExit(out var function)) return functionResult;
                var argumentsResult = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                return await function.FunctionReferenceValue.CallAsync(arguments.ArgumentsValue).ConfigureAwait(false);
            }
            case ExpressionKind.InitializerInvocation:
            {
                var exp = (IInitializerInvocationExpressionNode)expression;
                var argumentsResult = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                var initializerSymbol = exp.Initializer.ReferencedDeclaration!.Symbol.Assigned();
                return await CallInitializerAsync(initializerSymbol, arguments.ArgumentsValue).ConfigureAwait(false);
            }
            #endregion

            #region Name Expressions
            case ExpressionKind.VariableName:
            {
                var exp = (IVariableNameExpressionNode)expression;
                return variables[exp.ReferencedDefinition];
            }
            case ExpressionKind.Self:
            {
                var exp = (ISelfExpressionNode)expression;
                return variables[exp.ReferencedDefinition!];
            }
            case ExpressionKind.FunctionName:
            {
                var exp = (IFunctionNameExpressionNode)expression;
                return AzothValue.FunctionReference(new OrdinaryFunctionReference(this,
                    exp.ReferencedDeclaration!.Symbol.Assigned()));
            }
            case ExpressionKind.InitializerName:
            {
                var exp = (IInitializerNameExpressionNode)expression;
                var initializerSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                return AzothValue.FunctionReference(new InitializerReference(this, initializerSymbol));
            }
            #endregion

            #region Capability Expressions
            case ExpressionKind.Recovery:
            {
                var exp = (IRecoveryExpressionNode)expression;
                return await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
            }
            case ExpressionKind.ImplicitTempMove:
            {
                var exp = (IImplicitTempMoveExpressionNode)expression;
                return await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
            }
            case ExpressionKind.PrepareToReturn:
            {
                var exp = (IPrepareToReturnExpressionNode)expression;
                return await ExecuteAsync(exp.Value, variables).ConfigureAwait(false);
            }
            #endregion

            #region Async Expressions
            case ExpressionKind.AsyncBlock:
            {
                var exp = (IAsyncBlockExpressionNode)expression;
                var asyncScope = new AsyncScope();
                using var scope = variables.CreateNestedScope(asyncScope);
                try
                {
                    return await ExecuteAsync(exp.Block, scope);
                }
                finally
                {
                    await asyncScope.ExitAsync();
                }
            }
            case ExpressionKind.AsyncStart:
            {
                var exp = (IAsyncStartExpressionNode)expression;
                if (variables.AsyncScope is not AsyncScope asyncScope)
                    throw new InvalidOperationException(
                        "Cannot execute `go` or `do` expression outside of an async scope.");

                var task = exp.Scheduled
                    ? Task.Run(async () => await ExecuteAsync(exp.Expression!, variables))
                    : ExecuteAsync(exp.Expression!, variables).AsTask();

                asyncScope.Add(task);

                return AzothValue.Promise(task);
            }
            case ExpressionKind.Await:
            {
                var exp = (IAwaitExpressionNode)expression;
                var result = await ExecuteAsync(exp.Expression!, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return await value.PromiseValue.ConfigureAwait(false);
            }
            #endregion

            case ExpressionKind.Invalid:
                throw UnreachableInErrorFreeTree(expression);
            case ExpressionKind.NotTraversed:
                throw new UnreachableException($"Node type {expression.GetType().GetFriendlyName()} won't be traversed.");
        }
    }

    private static UnreachableException UnreachableInErrorFreeTree(IExpressionNode expression)
        => new($"Node type {expression.GetType().GetFriendlyName()} won't be in error free tree.");

    private static async ValueTask<AzothValue> ExecuteMatchAsync(
        AzothValue value,
        IPatternNode pattern,
        LocalVariables variables)
    {
        switch (pattern)
        {
            default:
                throw ExhaustiveMatch.Failed(pattern);
            case IBindingContextPatternNode pat:
                return await ExecuteMatchAsync(value, pat.Pattern, variables).ConfigureAwait(false);
            case IBindingPatternNode pat:
                variables.Add(pat, value);
                return AzothValue.Bool(true);
            case IOptionalPatternNode pat:
                if (value.IsNone)
                    return AzothValue.Bool(false);
                return await ExecuteMatchAsync(value, pat.Pattern, variables).ConfigureAwait(false);
        }
    }

    private async ValueTask<AzothValue> InitializeStringAsync(string value)
    {
        var bytes = new RawBoundedByteList(Encoding.UTF8.GetBytes(value));
        var arguments = new[]
        {
            // bytes: const Raw_Bounded_List[byte]
            AzothValue.RawBoundedList(bytes),
            // start: size
            AzothValue.Size(0),
            // byte_count: size
            AzothValue.Size(bytes.Count),
        };
        var layout = structLayouts.GetOrAdd(stringStruct, CreateStructLayout);
        var self = AzothValue.Struct(new(layout));
        return await CallInitializerAsync(stringInitializer, self, arguments).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> CallIntrinsicAsync(FunctionSymbol function, IReadOnlyList<AzothValue> arguments)
    {
        if (ReferenceEquals(function, Intrinsic.PrintRawUtf8Bytes))
        {
            string str = RawUtf8BytesToString(arguments);
            await standardOutputWriter.WriteAsync(str).ConfigureAwait(false);
            return AzothValue.None;
        }
        if (ReferenceEquals(function, Intrinsic.AbortRawUtf8Bytes))
        {
            string message = RawUtf8BytesToString(arguments);
            throw new Abort(message);
        }
        throw new NotImplementedException($"Intrinsic {function}");
    }

    private static string RawUtf8BytesToString(IReadOnlyList<AzothValue> arguments)
    {
        var bytes = (RawBoundedByteList)arguments[0].RawBoundedListValue;
        var start = arguments[1].SizeValue;
        var byteCount = arguments[2].SizeValue;
        var message = bytes.Utf8GetString(start, byteCount);
        return message;
    }

    private static ValueTask<AzothValue> CallIntrinsicAsync(InitializerSymbol initializer, IReadOnlyList<AzothValue> arguments)
    {
        if (ReferenceEquals(initializer, Intrinsic.InitRawBoundedList))
        {
            var listType = initializer.ContainingSymbol.TypeConstructor.ParameterTypes[0];
            nuint capacity = arguments[0].SizeValue;
            IRawBoundedList list;
            if (listType.Equals(Type.Byte))
                list = new RawBoundedByteList(capacity);
            else
                list = new RawBoundedList(capacity);
            return ValueTask.FromResult(AzothValue.RawBoundedList(list));
        }

        throw new NotImplementedException($"Intrinsic {initializer}");
    }

    private static ValueTask<AzothValue> CallIntrinsicAsync(
        MethodSymbol method,
        AzothValue self,
        IReadOnlyList<AzothValue> arguments)
    {
        if (ReferenceEquals(method, Intrinsic.GetRawBoundedListCapacity))
            return ValueTask.FromResult(AzothValue.Size(self.RawBoundedListValue.Capacity));
        if (ReferenceEquals(method, Intrinsic.GetRawBoundedListCount))
            return ValueTask.FromResult(AzothValue.Size(self.RawBoundedListValue.Count));
        if (ReferenceEquals(method, Intrinsic.RawBoundedListAdd))
        {
            self.RawBoundedListValue.Add(arguments[0]);
            return ValueTask.FromResult(AzothValue.None);
        }
        if (ReferenceEquals(method, Intrinsic.RawBoundedListAt))
            return ValueTask.FromResult(self.RawBoundedListValue.At(arguments[0].SizeValue));
        if (ReferenceEquals(method, Intrinsic.RawBoundedListSetAt))
        {
            self.RawBoundedListValue.Set(arguments[0].SizeValue, arguments[1]);
            return ValueTask.FromResult(AzothValue.None);
        }
        if (ReferenceEquals(method, Intrinsic.RawBoundedListShrink))
        {
            self.RawBoundedListValue.Shrink(arguments[0].SizeValue);
            return ValueTask.FromResult(AzothValue.None);
        }

        throw new NotImplementedException($"Intrinsic {method}");
    }

    private VTable CreateVTable(IClassDefinitionNode @class)
        => new(@class, methodSignatures, userTypes);

    private static StructLayout CreateStructLayout(IStructDefinitionNode @struct) => new(@struct);

    private async ValueTask<AzothResult> ExecuteArgumentsAsync(IFixedList<IExpressionNode> arguments, LocalVariables variables)
    {
        var values = new List<AzothValue>(arguments.Count);
        // Execute arguments in order
        foreach (var argument in arguments)
        {
            var result = await ExecuteAsync(argument, variables).ConfigureAwait(false);
            if (result.ShouldExit(out var value)) return result;
            values.Add(value);
        }

        return AzothValue.Arguments(values);
    }

    private async ValueTask<AzothResult> AddAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.Int(left.IntValue + right.IntValue);
        if (ReferenceEquals(plainType, PlainType.UInt)) return AzothValue.Int(left.IntValue + right.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)(left.I8Value + right.I8Value));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.Byte((byte)(left.ByteValue + right.ByteValue));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)(left.I16Value + right.I16Value));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.U16((ushort)(left.U16Value + right.U16Value));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(left.I32Value + right.I32Value);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.U32(left.U32Value + right.U32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(left.I64Value + right.I64Value);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.U64(left.U64Value + right.U64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(left.OffsetValue + right.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.Size(left.SizeValue + right.SizeValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.NInt(left.NIntValue + right.NIntValue);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return AzothValue.NUInt(left.NUIntValue + right.NUIntValue);
        throw new NotImplementedException($"Add {leftExp.Type.ToILString()}");
    }

    private async ValueTask<AzothResult> SubtractAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        // TODO check for negative values when subtracting unsigned
        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.Int(left.IntValue - right.IntValue);
        if (ReferenceEquals(plainType, PlainType.UInt)) return AzothValue.Int(left.IntValue - right.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)(left.I8Value - right.I8Value));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.Byte((byte)(left.ByteValue - right.ByteValue));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)(left.I16Value - right.I16Value));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.U16((ushort)(left.U16Value - right.U16Value));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(left.I32Value - right.I32Value);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.U32(left.U32Value - right.U32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(left.I64Value - right.I64Value);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.U64(left.U64Value - right.U64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(left.OffsetValue - right.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.Size(left.SizeValue - right.SizeValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.NInt(left.NIntValue - right.NIntValue);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return AzothValue.NUInt(left.NUIntValue - right.NUIntValue);
        throw new NotImplementedException($"Subtract {leftExp.Type.ToILString()}");
    }

    private async ValueTask<AzothResult> MultiplyAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.Int(left.IntValue * right.IntValue);
        if (ReferenceEquals(plainType, PlainType.UInt)) return AzothValue.Int(left.IntValue * right.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)(left.I8Value * right.I8Value));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.Byte((byte)(left.ByteValue * right.ByteValue));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)(left.I16Value * right.I16Value));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.U16((ushort)(left.U16Value * right.U16Value));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(left.I32Value * right.I32Value);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.U32(left.U32Value * right.U32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(left.I64Value * right.I64Value);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.U64(left.U64Value * right.U64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(left.OffsetValue * right.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.Size(left.SizeValue * right.SizeValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.NInt(left.NIntValue * right.NIntValue);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return AzothValue.NUInt(left.NUIntValue * right.NUIntValue);
        throw new NotImplementedException($"Multiply {leftExp.Type.ToILString()}");
    }

    private async ValueTask<AzothResult> DivideAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.Int(left.IntValue / right.IntValue);
        if (ReferenceEquals(plainType, PlainType.UInt)) return AzothValue.Int(left.IntValue / right.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)(left.I8Value / right.I8Value));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.Byte((byte)(left.ByteValue / right.ByteValue));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)(left.I16Value / right.I16Value));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.U16((ushort)(left.U16Value / right.U16Value));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(left.I32Value / right.I32Value);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.U32(left.U32Value / right.U32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(left.I64Value / right.I64Value);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.U64(left.U64Value / right.U64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(left.OffsetValue / right.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.Size(left.SizeValue / right.SizeValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.NInt(left.NIntValue / right.NIntValue);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return AzothValue.NUInt(left.NUIntValue / right.NUIntValue);
        throw new NotImplementedException($"Divide {leftExp.Type.ToILString()}");
    }

    private async ValueTask<AzothResult> BuiltInEqualsAsync(PlainType commonPlainType, IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        if (commonPlainType is OptionalPlainType optionalType)
        {
            if (left.IsNone && right.IsNone) return AzothValue.True;
            if (left.IsNone || right.IsNone) return AzothValue.False;
            return AzothValue.Bool(BuiltInEquals(optionalType.Referent, left, right));
        }

        return AzothValue.Bool(BuiltInEquals(commonPlainType, left, right));
    }

    private static bool BuiltInEquals(PlainType plainType, AzothValue left, AzothValue right)
    {
        if (ReferenceEquals(plainType, PlainType.Int)) return left.IntValue.Equals(right.IntValue);
        if (ReferenceEquals(plainType, PlainType.UInt)) return left.IntValue.Equals(right.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return left.I8Value.Equals(right.I8Value);
        if (ReferenceEquals(plainType, PlainType.Byte)) return left.ByteValue.Equals(right.ByteValue);
        if (ReferenceEquals(plainType, PlainType.Int16)) return left.I16Value.Equals(right.I16Value);
        if (ReferenceEquals(plainType, PlainType.UInt16)) return left.U16Value.Equals(right.U16Value);
        if (ReferenceEquals(plainType, PlainType.Int32)) return left.I32Value.Equals(right.I32Value);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return left.U32Value.Equals(right.U32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return left.I64Value.Equals(right.I64Value);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return left.U64Value.Equals(right.U64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return left.OffsetValue.Equals(right.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.Size)) return left.SizeValue.Equals(right.SizeValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return left.NIntValue.Equals(right.NIntValue);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return left.NUIntValue.Equals(right.NUIntValue);
        throw new NotImplementedException($"Compare equality of `{plainType}`.");
    }

    private async ValueTask<AzothResult> ReferenceEqualsAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var type = leftExp.Type.Known();
        if (type is OptionalType)
        {
            if (left.IsNone && right.IsNone) return AzothValue.True;
            if (left.IsNone || right.IsNone) return AzothValue.False;
        }

        return AzothValue.Bool(left.ObjectValue.ReferenceEquals(right.ObjectValue));
    }

    private async ValueTask<AzothResult> CompareAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var type = leftExp.Type;
        while (type is OptionalType optionalType)
        {
            if (left.IsNone && right.IsNone) return AzothValue.I32(0);
            if (left.IsNone || right.IsNone) throw new NotImplementedException("No comparison order");
            type = optionalType.Referent;
        }

        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.I32(left.IntValue.CompareTo(right.IntValue));
        if (ReferenceEquals(plainType, PlainType.UInt)) return AzothValue.I32(left.IntValue.CompareTo(right.IntValue));
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I32(left.I8Value.CompareTo(right.I8Value));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.I32(left.ByteValue.CompareTo(right.ByteValue));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I32(left.I16Value.CompareTo(right.I16Value));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.I32(left.U16Value.CompareTo(right.U16Value));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(left.I32Value.CompareTo(right.I32Value));
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.I32(left.U32Value.CompareTo(right.U32Value));
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I32(left.I64Value.CompareTo(right.I64Value));
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.I32(left.U64Value.CompareTo(right.U64Value));
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.I32(left.OffsetValue.CompareTo(right.OffsetValue));
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.I32(left.SizeValue.CompareTo(right.SizeValue));
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.I32(left.NIntValue.CompareTo(right.NIntValue));
        if (ReferenceEquals(plainType, PlainType.NUInt)) return AzothValue.I32(left.NUIntValue.CompareTo(right.NUIntValue));
        throw new NotImplementedException($"Compare `{type.ToILString()}`.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AzothResult Not(AzothResult result)
    {
        if (result.ShouldExit(out var value)) return result;
        return AzothValue.Bool(!value.BoolValue);
    }

    private async ValueTask<AzothResult> NegateAsync(IExpressionNode expression, LocalVariables variables)
    {
        var result = await ExecuteAsync(expression, variables).ConfigureAwait(false);
        if (result.ShouldExit(out var value)) return result;
        var type = expression.Type;
        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.Int(-value.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)-value.I8Value);
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)-value.I16Value);
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(-value.I32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(-value.I64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(-value.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.NInt(-value.NIntValue);
        if (type is CapabilityType { TypeConstructor: IntegerLiteralTypeConstructor }) return AzothValue.Int(-value.IntValue);
        throw new NotImplementedException($"Negate {type.ToILString()}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AzothValue IdentityHash(AzothValue value)
        => AzothValue.NUInt((nuint)value.ObjectValue.IdentityHash());

    private static AzothValue Remainder(
        AzothValue dividend,
        AzothValue divisor,
        CapabilityType type)
    {
        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return AzothValue.Int(dividend.IntValue % divisor.IntValue);
        if (ReferenceEquals(plainType, PlainType.UInt)) return AzothValue.Int(dividend.IntValue % divisor.IntValue);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)(dividend.I8Value % divisor.I8Value));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.Byte((byte)(dividend.ByteValue % divisor.ByteValue));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)(dividend.I16Value % divisor.I16Value));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.U16((ushort)(dividend.U16Value % divisor.U16Value));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(dividend.I32Value % divisor.I32Value);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.U32(dividend.U32Value % divisor.U32Value);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(dividend.I64Value % divisor.I64Value);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.U64(dividend.U64Value % divisor.U64Value);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(dividend.OffsetValue % divisor.OffsetValue);
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.Size(dividend.SizeValue % divisor.SizeValue);
        if (ReferenceEquals(plainType, PlainType.NInt)) return AzothValue.Offset(dividend.NIntValue % divisor.NIntValue);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return AzothValue.Size(dividend.NUIntValue % divisor.NUIntValue);
        throw new NotImplementedException($"Remainder {type.ToILString()}");
    }

    private async ValueTask<AzothValue> ToDisplayStringAsync(AzothValue value, CapabilityType type)
    {
        var plainType = type.PlainType;
        string displayString;
        if (ReferenceEquals(plainType, PlainType.Int)) displayString = value.IntValue.ToString();
        else if (ReferenceEquals(plainType, PlainType.UInt)) displayString = value.IntValue.ToString();
        else if (ReferenceEquals(plainType, PlainType.Byte)) displayString = value.ByteValue.ToString();
        else if (ReferenceEquals(plainType, PlainType.Int32)) displayString = value.I32Value.ToString();
        else if (ReferenceEquals(plainType, PlainType.UInt32)) displayString = value.U32Value.ToString();
        else if (ReferenceEquals(plainType, PlainType.Offset)) displayString = value.OffsetValue.ToString();
        else if (ReferenceEquals(plainType, PlainType.Size)) displayString = value.SizeValue.ToString();
        else if (ReferenceEquals(plainType, PlainType.NInt)) displayString = value.NIntValue.ToString();
        else if (ReferenceEquals(plainType, PlainType.NUInt)) displayString = value.NUIntValue.ToString();
        else throw new NotImplementedException($"to_display_string({type.ToILString()})");

        return await InitializeStringAsync(displayString).ConfigureAwait(false);
    }

    private async ValueTask<AzothResult> ExecuteBlockOrResultAsync(
        IBlockOrResultNode statement,
        LocalVariables variables)
        => statement switch
        {
            IBlockExpressionNode b => await ExecuteAsync(b, variables).ConfigureAwait(false),
            IResultStatementNode s => await ExecuteAsync(s, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(statement)
        };

    private async ValueTask<AzothResult> ExecuteElseAsync(
        IElseClauseNode elseClause,
        LocalVariables variables)
    {
        return elseClause switch
        {
            IBlockOrResultNode exp => await ExecuteBlockOrResultAsync(exp, variables).ConfigureAwait(false),
            IIfExpressionNode exp => await ExecuteAsync(exp, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(elseClause)
        };
    }

    private async ValueTask<AzothResult> ExecuteAssignmentAsync(
        IExpressionNode expression,
        AzothValue value,
        LocalVariables variables)
    {
        switch (expression)
        {
            default:
                throw new NotImplementedException($"Can't interpret assignment into {expression.GetType().Name}");
            case IVariableNameExpressionNode exp:
                variables[exp.ReferencedDefinition] = value;
                break;
            case IFieldAccessExpressionNode exp:
                var result = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var obj)) return result;
                // TODO handle the access operator
                obj.InstanceValue[exp.ReferencedDeclaration] = value;
                break;
        }

        return value;
    }

    public Task WaitForExitAsync() => executionTask;

    public TextReader StandardOutput { get; }
    public TextReader StandardError => TextReader.Null;

    public byte ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
}
