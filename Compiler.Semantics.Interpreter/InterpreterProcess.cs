using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Semantics.InterpreterHelpers;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using InlineMethod;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public sealed class InterpreterProcess
{
    public static InterpreterProcess StartEntryPoint(IPackageFacetNode packageFacet, IEnumerable<IPackageFacetNode> referencedPackageFacets)
    {
        if (packageFacet.EntryPoint is null)
            throw new ArgumentException("Cannot execute package without an entry point");

        return new InterpreterProcess(packageFacet, referencedPackageFacets, runTests: false);
    }

    public static InterpreterProcess StartTests(IPackageFacetNode packageFacet, IEnumerable<IPackageFacetNode> referencedPackageFacets)
        => new(packageFacet, referencedPackageFacets, runTests: true);

    public TimeSpan RunTime => runStopwatch.Elapsed;

    private readonly IPackageFacetNode packageFacet;
    private readonly Task executionTask;
    private readonly FrozenDictionary<FunctionSymbol, IFunctionInvocableDefinitionNode> functions;
    private readonly FrozenDictionary<MethodSymbol, IMethodDefinitionNode> valueMethods;
    private readonly FrozenDictionary<MethodSymbol, IMethodDefinitionNode> structMethods;
    private readonly FrozenDictionary<InitializerSymbol, IOrdinaryInitializerDefinitionNode?> initializers;
    private readonly FrozenDictionary<OrdinaryTypeSymbol, ITypeDefinitionNode> userTypes;
    private readonly IValueDefinitionNode? stringValue;
    private readonly IOrdinaryInitializerDefinitionNode? stringInitializer;
    private readonly BareType? stringBareType;
    private readonly IValueDefinitionNode? rangeValue;
    private readonly InitializerSymbol? rangeInitializer;
    private readonly BareType? rangeBareType;
    private byte? exitCode;
    internal TextWriter StandardOutputWriter { get; }
    private readonly MethodSignatureCache methodSignatures = new();
    private readonly ConcurrentDictionary<IClassDefinitionNode, ClassMetadata> classMetadata
        = new(ReferenceEqualityComparer.Instance);
    private readonly ConcurrentDictionary<IStructDefinitionNode, StructMetadata> structMetadata
        = new(ReferenceEqualityComparer.Instance);
    private readonly ConcurrentDictionary<IValueDefinitionNode, ValueMetadata> valueMetadata
        = new(ReferenceEqualityComparer.Instance);
    private readonly LocalVariables.Scope.Pool localVariableScopePool = new();
    private readonly Stopwatch runStopwatch = new();
    private readonly IntrinsicsRegistry intrinsics = IntrinsicsRegistry.Instance;

    private InterpreterProcess(IPackageFacetNode packageFacet, IEnumerable<IPackageFacetNode> referencedPackageFacets, bool runTests)
    {
        this.packageFacet = packageFacet;
        var allDefinitions = GetAllDefinitions(packageFacet, referencedPackageFacets);
        functions = allDefinitions
                    .OfType<IFunctionInvocableDefinitionNode>()
                    .ToFrozenDictionary(f => f.Symbol.Assigned());

        valueMethods = allDefinitions
                       .OfType<IMethodDefinitionNode>()
                       .Where(m => m.Symbol.Assigned().ContextTypeSymbol is OrdinaryTypeSymbol { Kind: TypeKind.Value })
                       .ToFrozenDictionary(m => m.Symbol.Assigned());

        structMethods = allDefinitions
                        .OfType<IMethodDefinitionNode>()
                        .Where(m => m.Symbol.Assigned().ContextTypeSymbol is OrdinaryTypeSymbol { Kind: TypeKind.Struct })
                        .ToFrozenDictionary(m => m.Symbol.Assigned());

        userTypes = allDefinitions.OfType<ITypeDefinitionNode>()
                                 .ToFrozenDictionary(c => c.Symbol);
        stringValue = userTypes.Values.OfType<IValueDefinitionNode>().Where(c => c.Symbol.Name == SpecialNames.StringTypeName).TrySingle();
        stringInitializer = stringValue?.Members.OfType<IOrdinaryInitializerDefinitionNode>().Single(c => c.Parameters.Count == 3);
        stringBareType = stringValue?.TypeConstructor.ConstructNullaryType(containingType: null);
        rangeValue = userTypes.Values.OfType<IValueDefinitionNode>().SingleOrDefault(c => c.Symbol.Name == SpecialNames.RangeTypeName);
        rangeInitializer = rangeValue?.Members.OfType<IInitializerDefinitionNode>().SingleOrDefault(c => c.Parameters.Count == 2)?.Symbol;
        rangeBareType = rangeValue?.TypeConstructor.ConstructNullaryType(containingType: null);

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
        StandardOutputWriter = new StreamWriter(pipe.Writer.AsStream(), Encoding.UTF8);
        StandardOutput = new StreamReader(pipe.Reader.AsStream(), Encoding.UTF8);

        executionTask = runTests ? Task.Run(RunTestsAsync) : Task.Run(CallEntryPointAsync);
    }

    private static IFixedList<IDefinitionNode> GetAllDefinitions(
        IPackageFacetNode packageFacet,
        IEnumerable<IPackageFacetNode> referencedPackageFacets)
        => GetAllDefinitions(referencedPackageFacets.Prepend(packageFacet).SelectMany(f => f.Definitions)).ToFixedList();

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
        await using var _ = StandardOutputWriter;

        var entryPoint = packageFacet.EntryPoint!;
        var parameterTypes = entryPoint.Symbol.Assigned().ParameterTypes;
        var arguments = new List<Value>(parameterTypes.Count);
        foreach (var parameterType in parameterTypes)
            arguments.Add(await InitializeMainParameterAsync(parameterType.Type));

        var returnValue = await CallFunctionAsync(entryPoint, arguments).ConfigureAwait(false);
        runStopwatch.Stop();

        // Flush any buffered output
        await StandardOutputWriter.FlushAsync().ConfigureAwait(false);
        var returnType = entryPoint.Symbol.Assigned().ReturnType;
        if (returnType.Equals(Type.Void))
            exitCode = 0;
        else if (returnType.Equals(Type.Byte))
            exitCode = returnValue.Byte;
        else
            throw new InvalidOperationException($"Main function cannot have return type {returnType.ToILString()}");
    }

    private async Task RunTestsAsync()
    {
        runStopwatch.Start();
        await using var _ = StandardOutputWriter;

        var testFunctions = packageFacet.Definitions.OfType<IFunctionDefinitionNode>()
                                        .Where(f => f.Attributes.Any(IsTestAttribute)).ToFixedSet();

        await StandardOutputWriter.WriteLineAsync($"Testing {packageFacet.PackageName} package...");
        await StandardOutputWriter.WriteLineAsync($"  Found {testFunctions.Count} tests");
        await StandardOutputWriter.WriteLineAsync();

        int failed = 0;

        var stopwatch = new Stopwatch();
        foreach (var function in testFunctions)
        {
            // TODO check that return type is void
            var symbol = function.Symbol;
            await StandardOutputWriter.WriteLineAsync($"{symbol.Assigned().ContainingSymbol.ToILString()}.{symbol.Assigned().Name} ...");
            try
            {
                stopwatch.Start();
                await CallFunctionAsync(function, []).ConfigureAwait(false);
                stopwatch.Stop();
                await StandardOutputWriter.WriteLineAsync($"  passed in {stopwatch.Elapsed.ToTotalSecondsAndMilliseconds()}");
            }
            catch (AbortException ex)
            {
                await StandardOutputWriter.WriteLineAsync("  FAILED: " + ex.Message);
                failed += 1;
            }
        }

        await StandardOutputWriter.WriteLineAsync();
        await StandardOutputWriter.WriteLineAsync($"Tested {packageFacet.PackageName} package");
        await StandardOutputWriter.WriteLineAsync($"{failed} failed tests out of {testFunctions.Count} total");

        // Flush any buffered output
        await StandardOutputWriter.FlushAsync().ConfigureAwait(false);

        runStopwatch.Stop();
    }

    private static bool IsTestAttribute(IAttributeNode attribute)
        => attribute.TypeName.ReferencedDeclaration!.Name.Text == "Test_Attribute";

    private ValueTask<Value> InitializeMainParameterAsync(Type parameterType)
    {
        if (parameterType is not CapabilityType { Arguments.Count: 0 } type)
            throw new InvalidOperationException(
                $"Parameter to main of type {parameterType.ToILString()} not supported");

        // TODO further restrict what can be passed to main
        // TODO support passing structs to main?
        var @class = userTypes.Values.OfType<IClassDefinitionNode>().Single(c => c.Symbol.TypeConstructor.Equals(type.TypeConstructor));
        var initializerSymbol = NoArgInitializerSymbol(@class);
        return CallInitializerAsync(type.BareType, initializerSymbol, []);
    }

    internal ValueTask<Value> CallFunctionAsync(FunctionSymbol functionSymbol, IReadOnlyList<Value> arguments)
    {
        if (intrinsics.Get(functionSymbol) is { } function)
            return function(this, functionSymbol, arguments);
        return CallFunctionAsync(functions[functionSymbol], arguments);
    }

    private async ValueTask<Value> CallFunctionAsync(
        IFunctionInvocableDefinitionNode function,
        IReadOnlyList<Value> arguments)
    {
        using var scope = localVariableScopePool.CreateRoot();
        var parameters = function.Parameters;
        // For loop avoid Linq Zip
        for (int i = 0; i < parameters.Count; i++)
            scope.Add(parameters[i], arguments[i]);

        var result = await ExecuteAsync(selfBareType: null, function.Body!.Statements, scope).ConfigureAwait(false);
        return result.ReturnValue;
    }

    internal ValueTask<Value> CallInitializerAsync(
        BareType selfBareType,
        InitializerSymbol initializerSymbol,
        IReadOnlyList<Value> arguments)
    {
        if (intrinsics.Get(initializerSymbol) is { } intrinsicInitializer)
            return intrinsicInitializer(selfBareType, initializerSymbol, arguments);
        var typeDefinition = userTypes[initializerSymbol.ContextTypeSymbol];
        return typeDefinition switch
        {
            IClassDefinitionNode @class => CallClassInitializerAsync(@class, selfBareType, initializerSymbol, arguments),
            IStructDefinitionNode @struct => CallStructInitializerAsync(@struct, selfBareType, initializerSymbol, arguments),
            IValueDefinitionNode value => CallValueInitializerAsync(value, selfBareType, initializerSymbol, arguments),
            ITraitDefinitionNode _ => throw new UnreachableException("Traits don't have initializers."),
            _ => throw ExhaustiveMatch.Failed(typeDefinition)
        };
    }

    private ValueTask<Value> CallClassInitializerAsync(
        IClassDefinitionNode @class,
        BareType selfBareType,
        InitializerSymbol initializerSymbol,
        IReadOnlyList<Value> arguments)
    {
        var metadata = classMetadata.GetOrAdd(@class, CreateClassMetadata);
        var self = Value.From(new ClassReference(metadata, selfBareType));
        return CallInitializerAsync(@class, initializerSymbol, self, selfBareType, arguments);
    }

    private static InitializerSymbol NoArgInitializerSymbol(IClassDefinitionNode baseClass)
        => baseClass.DefaultInitializer?.Symbol
           ?? baseClass.Members.OfType<IInitializerDefinitionNode>()
                       .Select(c => c.Symbol.Assigned()).Single(c => c.Arity == 0);

    private ValueTask<Value> CallStructInitializerAsync(
        IStructDefinitionNode @struct,
        BareType selfBareType,
        InitializerSymbol initializerSymbol,
        IReadOnlyList<Value> arguments)
    {
        var layout = structMetadata.GetOrAdd(@struct, CreateStructMetatdata);
        var self = Value.From((StructReference)new(layout));
        return CallInitializerAsync(@struct, initializerSymbol, self, selfBareType, arguments);
    }

    private ValueTask<Value> CallValueInitializerAsync(
        IValueDefinitionNode value,
        BareType selfBareType,
        InitializerSymbol initializerSymbol,
        IReadOnlyList<Value> arguments)
    {
        var layout = valueMetadata.GetOrAdd(value, CreateValueMetadata);
        var self = Value.From((ValueReference)new(layout));
        return CallInitializerAsync(value, initializerSymbol, self, selfBareType, arguments);
    }

    private ValueTask<Value> CallInitializerAsync(
        ITypeDefinitionNode typeDefinition,
        InitializerSymbol initializerSymbol,
        Value self,
        BareType selfBareType,
        IReadOnlyList<Value> arguments)
    {
        // TODO run field initializers
        var initializer = initializers[initializerSymbol];
        // Default initializer is null
        if (initializer is null) return CallDefaultInitializerAsync(typeDefinition, self, selfBareType);
        return CallInitializerAsync(initializer, self, selfBareType, arguments);
    }

    private async ValueTask<Value> CallInitializerAsync(
        IOrdinaryInitializerDefinitionNode initializer,
        Value self,
        BareType selfBareType,
        IReadOnlyList<Value> arguments)
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
                    self.InstanceReference[fieldParameter.ReferencedField!] = arguments[i];
                    break;
                case INamedParameterNode p:
                    scope.Add(p, arguments[i]);
                    break;
            }

        foreach (var statement in initializer.Body.Statements)
        {
            var result = await ExecuteAsync(selfBareType, statement, scope).ConfigureAwait(false);
            if (result.IsReturn) break;
            if (result.Type != ResultType.Ordinary) throw new UnreachableException("Unmatched break or next.");
        }
        return self;
    }

    /// <summary>
    /// Call the implicit default initializer for a type that has no initializers.
    /// </summary>
    private async ValueTask<Value> CallDefaultInitializerAsync(
        ITypeDefinitionNode typeDefinition,
        Value self,
        BareType selfBareType)
    {
        // Initialize fields to default values
        var fields = typeDefinition.Members.OfType<IFieldDefinitionNode>();
        foreach (var field in fields)
            self.InstanceReference[field] = Value.None;

        if (typeDefinition is IClassDefinitionNode
            {
                BaseTypeName.ReferencedDeclaration.Symbol: OrdinaryTypeSymbol baseClassSymbol
            })
        {
            var baseClass = (IClassDefinitionNode)userTypes[baseClassSymbol];
            var noArgConstructorSymbol = NoArgInitializerSymbol(baseClass);
            await CallInitializerAsync(baseClass, noArgConstructorSymbol, self, selfBareType, []);
        }

        return self;
    }

    internal ValueTask<Value> CallMethodAsync(
        MethodSymbol methodSymbol,
        Type selfType,
        Value self,
        IReadOnlyList<Value> arguments)
    {
        if (intrinsics.Get(methodSymbol) is { } method)
            return method(methodSymbol, self, arguments);
        // This is not part of the intrinsics because it is actually built-in and won't be declared with #Intrinsic
        if (ReferenceEquals(methodSymbol, Primitive.IdentityHash))
            return ValueTask.FromResult(IdentityHash(self));

        switch (selfType)
        {
            case CapabilityType t:
                return CallMethodAsync(methodSymbol, t.BareType, self, arguments);
            case SelfViewpointType t:
                return CallMethodAsync(methodSymbol, t.Referent, self, arguments);
            case CapabilitySetSelfType t:
                return CallMethodAsync(methodSymbol, t.BareType, self, arguments);
            case CapabilityViewpointType t:
                return CallMethodAsync(methodSymbol, t.Referent, self, arguments);
            case CapabilitySetRestrictedType t:
                return CallMethodAsync(methodSymbol, t.Referent, self, arguments);
            case VoidType _:
            case NeverType _:
            case OptionalType _:
            case GenericParameterType _:
            case FunctionType _:
                var methodSignature = methodSignatures[methodSymbol];
                throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}");
            default:
                throw ExhaustiveMatch.Failed(selfType);
        }
    }

    private ValueTask<Value> CallMethodAsync(
        MethodSymbol methodSymbol,
        BareType selfType,
        Value self,
        IReadOnlyList<Value> arguments)
    {
        // TODO null is an odd case, generic instantiation should avoid it but this works for now
        var semantics = selfType.Semantics ?? (self.InstanceReference.IsClassReference ? TypeSemantics.Reference : TypeSemantics.Value);
        return semantics switch
        {
            TypeSemantics.Value => CallValueMethod(methodSymbol, selfType, self, arguments),
            TypeSemantics.Hybrid => CallStructMethod(methodSymbol, selfType, self, arguments),
            TypeSemantics.Reference => CallClassMethodAsync(methodSymbol, selfType, self, arguments),
            _ => throw ExhaustiveMatch.Failed(semantics),
        };
    }

    private ValueTask<Value> CallClassMethodAsync(
        MethodSymbol methodSymbol,
        BareType selfType,
        Value self,
        IReadOnlyList<Value> arguments)
    {
        var methodSignature = methodSignatures[methodSymbol];
        var vtable = self.ClassReference.ClassMetadata;
        var method = vtable[methodSignature];
        return CallMethodAsync(method, self, selfType, arguments);
    }

    private ValueTask<Value> CallValueMethod(
        MethodSymbol methodSymbol,
        BareType selfType,
        Value self,
        IReadOnlyList<Value> arguments)
    {
        return methodSymbol.Name.Text switch
        {
            "remainder" => ValueTask.FromResult(Remainder(self, arguments.Single(), selfType)),
            "to_display_string" => ToDisplayStringAsync(self, selfType),
            _ => CallMethodAsync(valueMethods[methodSymbol], self, selfType, arguments),
        };
    }

    private ValueTask<Value> CallStructMethod(
        MethodSymbol methodSymbol,
        BareType selfType,
        Value self,
        IReadOnlyList<Value> arguments)
    {
        return methodSymbol.Name.Text switch
        {
            "remainder" => ValueTask.FromResult(Remainder(self, arguments.Single(), selfType)),
            "to_display_string" => ToDisplayStringAsync(self, selfType),
            _ => CallMethodAsync(structMethods[methodSymbol], self, selfType, arguments),
        };
    }

    private async ValueTask<Value> CallMethodAsync(
        IMethodDefinitionNode method,
        Value self,
        BareType selfBareType,
        IReadOnlyList<Value> arguments)
    {
        if (method.Body is null)
            throw new InvalidOperationException($"Can't call abstract method {method.Syntax}");

        using var scope = localVariableScopePool.CreateRoot();
        scope.Add(method.SelfParameter, self);
        var parameters = method.Parameters; // Avoids repeated access
        // For loop avoid Linq Zip
        for (int i = 0; i < parameters.Count; i++)
            scope.Add(parameters[i], arguments[i]);

        var result = await ExecuteAsync(selfBareType, method.Body.Statements, scope).ConfigureAwait(false);
        return result.ReturnValue;
    }

    private async ValueTask<Result> ExecuteAsync(
        BareType? selfBareType,
        IFixedList<IStatementNode> statements,
        LocalVariables variables)
    {
        foreach (var statement in statements)
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IBodyStatementNode bodyStatement:
                    var result = await ExecuteAsync(selfBareType, bodyStatement, variables).ConfigureAwait(false);
                    if (result.ShouldExit()) return result;
                    break;
                case IResultStatementNode resultStatement:
                    return await ExecuteAsync(selfBareType, resultStatement.Expression!, variables).ConfigureAwait(false);
            }

        return Value.None;
    }

    private async ValueTask<Result> ExecuteAsync(
        BareType? selfBareType,
        IBodyStatementNode statement,
        LocalVariables variables)
    {
        switch (statement)
        {
            default:
                throw ExhaustiveMatch.Failed(statement);
            case IExpressionStatementNode s:
                return await ExecuteAsync(selfBareType, s.Expression!, variables).ConfigureAwait(false);
            case IVariableDeclarationStatementNode d:
            {
                var initialValueResult = d.Initializer is { } initializer
                    ? await ExecuteAsync(selfBareType, initializer, variables).ConfigureAwait(false)
                    : Value.None;
                if (initialValueResult.ShouldExit(out var initialValue)) return initialValueResult;
                variables.Add(d, initialValue);
                return Value.None;
            }
        }
    }

    private ValueTask<Result> ExecuteAsync(BareType? selfBareType, IResultStatementNode statement, LocalVariables variables)
        => ExecuteAsync(selfBareType, statement.Expression!, variables);

    private async ValueTask<Result> ExecuteAsync(
        BareType? selfBareType,
        IExpressionNode expression,
        LocalVariables variables)
    {
        // A switch on type compiles to essentially a long if/else change of `is Type t` pattern
        // matches. It was thought that this code spends most of its time determining the node type.
        // To avoid type checks, this code now switches on an enum of node types. However, that
        // change as not led to any appreciable performance improvement. The profiler still shows
        // that the interpreter spends most of its time in this method.

        switch (expression.ExpressionKind)
        {
            default:
                throw ExhaustiveMatch.Failed(expression.ExpressionKind);

            #region Expressions
            case ExpressionKind.Block:
            {
                var block = (IBlockExpressionNode)expression;
                using var scope = variables.CreateNestedScope();
                return await ExecuteAsync(selfBareType, block.Statements, scope).ConfigureAwait(false);
            }
            case ExpressionKind.Unsafe:
            {
                var exp = (IUnsafeExpressionNode)expression;
                return await ExecuteAsync(selfBareType, exp.Expression!, variables).ConfigureAwait(false);
            }
            #endregion

            #region Instance Member Access Expressions
            case ExpressionKind.FieldAccess:
            {
                var exp = (IFieldAccessExpressionNode)expression;
                var result = await ExecuteAsync(selfBareType, exp.Context, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var obj)) return result;
                return obj.InstanceReference[exp.ReferencedDeclaration];
            }
            case ExpressionKind.MethodAccess:
            {
                var exp = (IMethodAccessExpressionNode)expression;
                var context = exp.Context; // Avoids repeated access
                var selfResult = await ExecuteAsync(selfBareType, context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var methodSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = context.Type.Known();
                return Value.From(new MethodReference(this, selfType, self, methodSymbol));
            }
            #endregion

            #region Literal Expressions
            case ExpressionKind.BoolLiteral:
            {
                var exp = (IBoolLiteralExpressionNode)expression;
                return Value.From(exp.Value);
            }
            case ExpressionKind.IntegerLiteral:
            {
                var exp = (IIntegerLiteralExpressionNode)expression;
                return Value.From(exp.Value);
            }
            case ExpressionKind.NoneLiteral:
                return Value.None;
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
                var leftOperand = exp.LeftOperand!; // Avoids repeated access
                var result = exp.Operator switch
                {
                    AssignmentOperator.Simple =>
                        // TODO the expression being assigned into is supposed to be evaluated first
                        await ExecuteAsync(selfBareType, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Plus
                        => await AddAsync(selfBareType, leftOperand, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Minus
                        => await SubtractAsync(selfBareType, leftOperand, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Asterisk
                        => await MultiplyAsync(selfBareType, leftOperand, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Slash
                        => await DivideAsync(selfBareType, leftOperand, exp.RightOperand!, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
                if (result.ShouldExit(out var value)) return result;
                return await ExecuteAssignmentAsync(selfBareType, leftOperand, value, variables).ConfigureAwait(false);
            }
            case ExpressionKind.BinaryOperator:
            {
                var exp = (IBinaryOperatorExpressionNode)expression;
                var binaryOperator = exp.Operator;
                switch (binaryOperator)
                {
                    default:
                        throw ExhaustiveMatch.Failed();
                    case BinaryOperator.Plus:
                        return await AddAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.Minus:
                        return await SubtractAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Asterisk:
                        return await MultiplyAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Slash:
                        return await DivideAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.EqualsEquals:
                        return await BuiltInEqualsAsync(selfBareType, exp.NumericOperatorCommonPlainType!, exp.LeftOperand!,
                            exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.NotEqual:
                        return Not(await BuiltInEqualsAsync(selfBareType, exp.NumericOperatorCommonPlainType!, exp.LeftOperand!,
                            exp.RightOperand!, variables).ConfigureAwait(false));
                    case BinaryOperator.ReferenceEquals:
                        return await ReferenceEqualsAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.NotReferenceEqual:
                        return Not(await ReferenceEqualsAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.LessThan:
                    {
                        var result = await CompareAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return Value.From(value.I32 < 0);
                    }
                    case BinaryOperator.LessThanOrEqual:
                    {
                        var result = await CompareAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return Value.From(value.I32 <= 0);
                    }
                    case BinaryOperator.GreaterThan:
                    {
                        var result = await CompareAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return Value.From(value.I32 > 0);
                    }
                    case BinaryOperator.GreaterThanOrEqual:
                    {
                        var result = await CompareAsync(selfBareType, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                        if (result.ShouldExit(out var value)) return result;
                        return Value.From(value.I32 >= 0);
                    }
                    case BinaryOperator.And:
                    {
                        var leftResult = await ExecuteAsync(selfBareType, exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (!left.Bool) return Value.False;
                        return await ExecuteAsync(selfBareType, exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.Or:
                    {
                        var leftResult = await ExecuteAsync(selfBareType, exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (left.Bool) return Value.True;
                        return await ExecuteAsync(selfBareType, exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.DotDot:
                    case BinaryOperator.LessThanDotDot:
                    case BinaryOperator.DotDotLessThan:
                    case BinaryOperator.LessThanDotDotLessThan:
                    {
                        var leftResult = await ExecuteAsync(selfBareType, exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (!binaryOperator.RangeInclusiveOfStart()) left = left.IncrementInt();
                        var rightResult = await ExecuteAsync(selfBareType, exp.RightOperand!, variables).ConfigureAwait(false);
                        if (rightResult.ShouldExit(out var right)) return rightResult;
                        if (binaryOperator.RangeInclusiveOfEnd()) right = right.IncrementInt();
                        return await CallValueInitializerAsync(rangeValue!, rangeBareType!, rangeInitializer!, [left, right]);
                    }
                    case BinaryOperator.QuestionQuestion:
                    {
                        var leftResult = await ExecuteAsync(selfBareType, exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (leftResult.ShouldExit(out var left)) return leftResult;
                        if (!left.IsNone) return left;
                        return await ExecuteAsync(selfBareType, exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                }
            }
            case ExpressionKind.UnaryOperator:
            {
                var exp = (IUnaryOperatorExpressionNode)expression;
                return exp.Operator switch
                {
                    UnaryOperator.Not => Not(await ExecuteAsync(selfBareType, exp.Operand!, variables).ConfigureAwait(false)),
                    UnaryOperator.Minus => await NegateAsync(selfBareType, exp.Operand!, variables).ConfigureAwait(false),
                    UnaryOperator.Plus => await ExecuteAsync(selfBareType, exp.Operand!, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
            }
            case ExpressionKind.Conversion:
            {
                var exp = (IConversionExpressionNode)expression;
                var referent = exp.Referent!; // Avoids repeated access
                var result = await ExecuteAsync(selfBareType, referent, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return value.Convert(referent.Type.Known(), (CapabilityType)exp.ConvertToType.NamedType, false);
            }
            case ExpressionKind.ImplicitConversion:
            {
                var exp = (IImplicitConversionExpressionNode)expression;
                var referent = exp.Referent;
                var result = await ExecuteAsync(selfBareType, referent, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return value.Convert(referent.Type.Known(), (CapabilityType)exp.Type, true);
            }
            case ExpressionKind.OptionalConversionExpression:
            {
                var exp = (IOptionalConversionExpressionNode)expression;
                var referent = exp.Referent;
                var result = await ExecuteAsync(selfBareType, referent, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                // TODO handle real conversion for `T??` etc.
                return value;
            }
            case ExpressionKind.PatternMatch:
            {
                var exp = (IPatternMatchExpressionNode)expression;
                var result = await ExecuteAsync(selfBareType, exp.Referent!, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return await ExecuteMatchAsync(selfBareType, value, exp.Pattern, variables);
            }
            #endregion

            #region Control Flow Expressions
            case ExpressionKind.If:
            {
                var exp = (IIfExpressionNode)expression;
                var conditionResult = await ExecuteAsync(selfBareType, exp.Condition!, variables).ConfigureAwait(false);
                if (conditionResult.ShouldExit(out var condition)) return conditionResult;
                if (condition.Bool)
                    return await ExecuteBlockOrResultAsync(selfBareType, exp.ThenBlock, variables).ConfigureAwait(false);
                if (exp.ElseClause is { } elseClause)
                    return await ExecuteElseAsync(selfBareType, elseClause, variables).ConfigureAwait(false);
                return Value.None;
            }
            case ExpressionKind.Loop:
            {
                var exp = (ILoopExpressionNode)expression;
                var block = exp.Block; // Avoids repeated access
                while (true)
                {
                    var result = await ExecuteAsync(selfBareType, block, variables).ConfigureAwait(false);
                    switch (result.Type)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(result.Type);
                        case ResultType.Next:
                        case ResultType.Ordinary:
                            continue;
                        case ResultType.Break:
                            return result.Value;
                        case ResultType.Return:
                            return result;
                    }
                }
            }
            case ExpressionKind.While:
            {
                var exp = (IWhileExpressionNode)expression;
                var conditionExpression = exp.Condition!; // Avoids repeated access
                var block = exp.Block; // Avoids repeated access
                while (true)
                {
                    // Create a variable scope in case a variable is declared by a pattern in the condition
                    using var scope = variables.CreateNestedScope();
                    var conditionResult = await ExecuteAsync(selfBareType, conditionExpression, scope).ConfigureAwait(false);
                    if (conditionResult.ShouldExit(out var condition)) return conditionResult;
                    if (!condition.Bool)
                        return Value.None;

                    var result = await ExecuteAsync(selfBareType, block, scope).ConfigureAwait(false);
                    switch (result.Type)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(result.Type);
                        case ResultType.Next:
                        case ResultType.Ordinary:
                            continue;
                        case ResultType.Break:
                            return result.Value;
                        case ResultType.Return:
                            return result;
                    }
                }
            }
            case ExpressionKind.Foreach:
            {
                var exp = (IForeachExpressionNode)expression;
                var inExpression = exp.InExpression!; // Avoids repeated access
                var iterableResult = await ExecuteAsync(selfBareType, inExpression, variables).ConfigureAwait(false);
                if (iterableResult.ShouldExit(out var iterable)) return iterableResult;
                IBindingNode loopVariable = exp;
                // Call `iterable.iterate()` if it exists
                Value iterator;
                CapabilityType iteratorType;
                if (exp.ReferencedIterateMethod is { } iterateMethod)
                {
                    var selfType = (CapabilityType)inExpression.Type;
                    var methodSymbol = iterateMethod.Symbol!;
                    iterator = await CallMethodAsync(methodSymbol, selfType, iterable, []).ConfigureAwait(false);
                    iteratorType = (CapabilityType)methodSymbol.ReturnType;
                }
                else
                {
                    iterator = iterable;
                    iteratorType = (CapabilityType)inExpression.Type;
                }

                var nextMethod = exp.ReferencedNextMethod!.Symbol!; // Avoids repeated access
                var block = exp.Block; // Avoids repeated access
                while (true)
                {
                    var value = await CallMethodAsync(nextMethod, iteratorType, iterator, []).ConfigureAwait(false);
                    if (value.IsNone) break;

                    using var scope = variables.CreateNestedScope();
                    scope.Add(loopVariable, value);
                    var result = await ExecuteAsync(selfBareType, block, scope).ConfigureAwait(false);
                    switch (result.Type)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(result.Type);
                        case ResultType.Next:
                        case ResultType.Ordinary:
                            continue;
                        case ResultType.Break:
                            return result.Value;
                        case ResultType.Return:
                            return result;
                    }
                }

                return Value.None;
            }
            case ExpressionKind.Break:
            {
                var exp = (IBreakExpressionNode)expression;
                if (exp.Value is not { } value) return Result.BreakWithoutValue;
                var result = await ExecuteAsync(selfBareType, value, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var breakValue)) return result;
                return Result.Break(breakValue);
            }
            case ExpressionKind.Next:
                return Result.Next;
            case ExpressionKind.Return:
            {
                var exp = (IReturnExpressionNode)expression;
                if (exp.Value is not { } value) return Result.ReturnVoid;
                var result = await ExecuteAsync(selfBareType, value, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var returnValue)) return result;
                return Result.Return(returnValue);
            }
            #endregion

            #region Invocation Expressions
            case ExpressionKind.FunctionInvocation:
            {
                var exp = (IFunctionInvocationExpressionNode)expression;
                var argumentsResult = await ExecuteArgumentsAsync(selfBareType, exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                var functionSymbol = exp.Function.ReferencedDeclaration!.Symbol.Assigned();
                return await CallFunctionAsync(functionSymbol, arguments.Arguments).ConfigureAwait(false);
            }
            case ExpressionKind.MethodInvocation:
            {
                var exp = (IMethodInvocationExpressionNode)expression;
                var method = exp.Method; // Avoids repeated access
                var context = method.Context; // Avoids repeated access
                var selfResult = await ExecuteAsync(selfBareType, context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var argumentsResult = await ExecuteArgumentsAsync(selfBareType, exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                var methodSymbol = method.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = context.Type.Known();
                return await CallMethodAsync(methodSymbol, selfType, self, arguments.Arguments).ConfigureAwait(false);
            }
            case ExpressionKind.GetterInvocation:
            {
                var exp = (IGetterInvocationExpressionNode)expression;
                var context = exp.Context; // Avoids repeated access
                var selfResult = await ExecuteAsync(selfBareType, context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var getterSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = context.Type.Known();
                return await CallMethodAsync(getterSymbol, selfType, self, []).ConfigureAwait(false);
            }
            case ExpressionKind.SetterInvocation:
            {
                var exp = (ISetterInvocationExpressionNode)expression;
                var context = exp.Context; // Avoids repeated access
                var selfResult = await ExecuteAsync(selfBareType, context, variables).ConfigureAwait(false);
                if (selfResult.ShouldExit(out var self)) return selfResult;
                var valueResult = await ExecuteAsync(selfBareType, exp.Value!, variables).ConfigureAwait(false);
                if (valueResult.ShouldExit(out var value)) return valueResult;
                var setterSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var selfType = context.Type.Known();
                return await CallMethodAsync(setterSymbol, selfType, self, [value]).ConfigureAwait(false);
            }
            case ExpressionKind.FunctionReferenceInvocation:
            {
                var exp = (IFunctionReferenceInvocationExpressionNode)expression;
                var functionResult = await ExecuteAsync(selfBareType, exp.Expression, variables).ConfigureAwait(false);
                if (functionResult.ShouldExit(out var function)) return functionResult;
                var argumentsResult = await ExecuteArgumentsAsync(selfBareType, exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                return await function.FunctionReference.CallAsync(arguments.Arguments).ConfigureAwait(false);
            }
            case ExpressionKind.InitializerInvocation:
            {
                var exp = (IInitializerInvocationExpressionNode)expression;
                var argumentsResult = await ExecuteArgumentsAsync(selfBareType, exp.Arguments!, variables).ConfigureAwait(false);
                if (argumentsResult.ShouldExit(out var arguments)) return argumentsResult;
                var initializer = exp.Initializer; // Avoids repeated access
                var bareType = initializer.Context.NamedBareType!;
                var initializerSymbol = initializer.ReferencedDeclaration!.Symbol.Assigned();
                return await CallInitializerAsync(bareType, initializerSymbol, arguments.Arguments).ConfigureAwait(false);
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
                return Value.From(new OrdinaryFunctionReference(this,
                    exp.ReferencedDeclaration!.Symbol.Assigned()));
            }
            case ExpressionKind.InitializerName:
            {
                var exp = (IInitializerNameExpressionNode)expression;
                var bareType = exp.Context.NamedBareType!;
                var initializerSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                return Value.From(new InitializerReference(this, bareType, initializerSymbol));
            }
            #endregion

            #region Capability Expressions
            case ExpressionKind.Recovery:
            {
                var exp = (IRecoveryExpressionNode)expression;
                return await ExecuteAsync(selfBareType, exp.Referent, variables).ConfigureAwait(false);
            }
            case ExpressionKind.ImplicitTempMove:
            {
                var exp = (IImplicitTempMoveExpressionNode)expression;
                return await ExecuteAsync(selfBareType, exp.Referent, variables).ConfigureAwait(false);
            }
            case ExpressionKind.PrepareToReturn:
            {
                var exp = (IPrepareToReturnExpressionNode)expression;
                return await ExecuteAsync(selfBareType, exp.Value, variables).ConfigureAwait(false);
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
                    return await ExecuteAsync(selfBareType, exp.Block, scope);
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
                    ? Task.Run(async () => await ExecuteAsync(selfBareType, exp.Expression!, variables))
                    : ExecuteAsync(selfBareType, exp.Expression!, variables).AsTask();

                asyncScope.Add(task);

                return Value.From(task);
            }
            case ExpressionKind.Await:
            {
                var exp = (IAwaitExpressionNode)expression;
                var result = await ExecuteAsync(selfBareType, exp.Expression!, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var value)) return result;
                return await value.Promise.ConfigureAwait(false);
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

    private static async ValueTask<Value> ExecuteMatchAsync(
        BareType? selfBareType,
        Value value,
        IPatternNode pattern,
        LocalVariables variables)
    {
        switch (pattern)
        {
            default:
                throw ExhaustiveMatch.Failed(pattern);
            case ITypePatternNode pat:
                return Value.From(value.IsOfType(pat.Type.NamedType.Known(), selfBareType));
            case IBindingContextPatternNode pat:
                if (pat.Type is { } type && !value.IsOfType(type.NamedType.Known(), selfBareType))
                    return Value.False;
                return await ExecuteMatchAsync(selfBareType, value, pat.Pattern, variables).ConfigureAwait(false);
            case IBindingPatternNode pat:
                variables.Add(pat, value);
                return Value.True;
            case IOptionalPatternNode pat:
                if (value.IsNone)
                    return Value.False;
                return await ExecuteMatchAsync(selfBareType, value, pat.Pattern, variables).ConfigureAwait(false);
        }
    }

    private async ValueTask<Value> InitializeStringAsync(string value)
    {
        var bytes = RawHybridBoundedList.Create(value);
        var arguments = new[]
        {
            // bytes: const Raw_Bounded_List[byte]
            Value.From(bytes),
            // start: size
            Value.FromSize(0),
            // byte_count: size
            Value.FromSize(bytes.Count),
        };
        if (stringValue is null)
            throw new Exception("Cannot initialize string literal because no string type was found.");
        var layout = valueMetadata.GetOrAdd(stringValue, CreateValueMetadata);
        var self = Value.From(new ValueReference(layout));
        return await CallInitializerAsync(stringInitializer!, self, stringBareType!, arguments).ConfigureAwait(false);
    }

    private ClassMetadata CreateClassMetadata(IClassDefinitionNode @class)
        => new(@class, methodSignatures, userTypes);

    private static StructMetadata CreateStructMetatdata(IStructDefinitionNode @struct) => new(@struct);

    private static ValueMetadata CreateValueMetadata(IValueDefinitionNode value) => new(value);

    private async ValueTask<Result> ExecuteArgumentsAsync(
        BareType? selfBareType,
        IFixedList<IExpressionNode> arguments,
        LocalVariables variables)
    {
        var values = new List<Value>(arguments.Count);
        // Execute arguments in order
        foreach (var argument in arguments)
        {
            var result = await ExecuteAsync(selfBareType, argument, variables).ConfigureAwait(false);
            if (result.ShouldExit(out var value)) return result;
            values.Add(value);
        }

        return Value.FromArguments(values);
    }

    private async ValueTask<Result> AddAsync(
        BareType? selfBareType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.From(left.Int + right.Int);
        if (ReferenceEquals(plainType, PlainType.UInt)) return Value.From(left.Int + right.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)(left.I8 + right.I8));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromByte((byte)(left.Byte + right.Byte));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)(left.I16 + right.I16));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromU16((ushort)(left.U16 + right.U16));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(left.I32 + right.I32);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromU32(left.U32 + right.U32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(left.I64 + right.I64);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromU64(left.U64 + right.U64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(left.Offset + right.Offset);
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromSize(left.Size + right.Size);
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromNInt(left.NInt + right.NInt);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return Value.FromNUInt(left.NUInt + right.NUInt);
        throw new NotImplementedException($"Add {leftExp.Type.ToILString()}");
    }

    private async ValueTask<Result> SubtractAsync(
        BareType? selfBareType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        // TODO check for negative values when subtracting unsigned
        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.From(left.Int - right.Int);
        if (ReferenceEquals(plainType, PlainType.UInt)) return Value.From(left.Int - right.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)(left.I8 - right.I8));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromByte((byte)(left.Byte - right.Byte));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)(left.I16 - right.I16));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromU16((ushort)(left.U16 - right.U16));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(left.I32 - right.I32);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromU32(left.U32 - right.U32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(left.I64 - right.I64);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromU64(left.U64 - right.U64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(left.Offset - right.Offset);
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromSize(left.Size - right.Size);
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromNInt(left.NInt - right.NInt);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return Value.FromNUInt(left.NUInt - right.NUInt);
        throw new NotImplementedException($"Subtract {leftExp.Type.ToILString()}");
    }

    private async ValueTask<Result> MultiplyAsync(
        BareType? selfBareType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.From(left.Int * right.Int);
        if (ReferenceEquals(plainType, PlainType.UInt)) return Value.From(left.Int * right.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)(left.I8 * right.I8));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromByte((byte)(left.Byte * right.Byte));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)(left.I16 * right.I16));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromU16((ushort)(left.U16 * right.U16));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(left.I32 * right.I32);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromU32(left.U32 * right.U32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(left.I64 * right.I64);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromU64(left.U64 * right.U64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(left.Offset * right.Offset);
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromSize(left.Size * right.Size);
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromNInt(left.NInt * right.NInt);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return Value.FromNUInt(left.NUInt * right.NUInt);
        throw new NotImplementedException($"Multiply {leftExp.Type.ToILString()}");
    }

    private async ValueTask<Result> DivideAsync(
        BareType? selfBareType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var plainType = leftExp.Type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.From(left.Int / right.Int);
        if (ReferenceEquals(plainType, PlainType.UInt)) return Value.From(left.Int / right.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)(left.I8 / right.I8));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromByte((byte)(left.Byte / right.Byte));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)(left.I16 / right.I16));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromU16((ushort)(left.U16 / right.U16));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(left.I32 / right.I32);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromU32(left.U32 / right.U32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(left.I64 / right.I64);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromU64(left.U64 / right.U64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(left.Offset / right.Offset);
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromSize(left.Size / right.Size);
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromNInt(left.NInt / right.NInt);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return Value.FromNUInt(left.NUInt / right.NUInt);
        throw new NotImplementedException($"Divide {leftExp.Type.ToILString()}");
    }

    private async ValueTask<Result> BuiltInEqualsAsync(
        BareType? selfBareType,
        PlainType commonPlainType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        if (commonPlainType is OptionalPlainType optionalType)
        {
            if (left.IsNone && right.IsNone) return Value.True;
            if (left.IsNone || right.IsNone) return Value.False;
            return Value.From(BuiltInEquals(optionalType.Referent, left, right));
        }

        return Value.From(BuiltInEquals(commonPlainType, left, right));
    }

    private static bool BuiltInEquals(PlainType plainType, Value left, Value right)
    {
        if (ReferenceEquals(plainType, PlainType.Int)) return left.Int.Equals(right.Int);
        if (ReferenceEquals(plainType, PlainType.UInt)) return left.Int.Equals(right.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return left.I8.Equals(right.I8);
        if (ReferenceEquals(plainType, PlainType.Byte)) return left.Byte.Equals(right.Byte);
        if (ReferenceEquals(plainType, PlainType.Int16)) return left.I16.Equals(right.I16);
        if (ReferenceEquals(plainType, PlainType.UInt16)) return left.U16.Equals(right.U16);
        if (ReferenceEquals(plainType, PlainType.Int32)) return left.I32.Equals(right.I32);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return left.U32.Equals(right.U32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return left.I64.Equals(right.I64);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return left.U64.Equals(right.U64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return left.Offset.Equals(right.Offset);
        if (ReferenceEquals(plainType, PlainType.Size)) return left.Size.Equals(right.Size);
        if (ReferenceEquals(plainType, PlainType.NInt)) return left.NInt.Equals(right.NInt);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return left.NUInt.Equals(right.NUInt);
        throw new NotImplementedException($"Compare equality of `{plainType}`.");
    }

    private async ValueTask<Result> ReferenceEqualsAsync(
        BareType? selfBareType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var type = leftExp.Type.Known();
        if (type is OptionalType)
        {
            if (left.IsNone && right.IsNone) return Value.True;
            if (left.IsNone || right.IsNone) return Value.False;
        }

        return Value.From(left.ClassReference.ReferenceEquals(right.ClassReference));
    }

    private async ValueTask<Result> CompareAsync(
        BareType? selfBareType,
        IExpressionNode leftExp,
        IExpressionNode rightExp,
        LocalVariables variables)
    {
        // Don't check types match to avoid the overhead since the compiler should enforce this

        var leftResult = await ExecuteAsync(selfBareType, leftExp, variables).ConfigureAwait(false);
        if (leftResult.ShouldExit(out var left)) return leftResult;
        var rightResult = await ExecuteAsync(selfBareType, rightExp, variables).ConfigureAwait(false);
        if (rightResult.ShouldExit(out var right)) return leftResult;
        var type = leftExp.Type;
        while (type is OptionalType optionalType)
        {
            if (left.IsNone && right.IsNone) return Value.FromI32(0);
            if (left.IsNone || right.IsNone) throw new NotImplementedException("No comparison order");
            type = optionalType.Referent;
        }

        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.FromI32(left.Int.CompareTo(right.Int));
        if (ReferenceEquals(plainType, PlainType.UInt)) return Value.FromI32(left.Int.CompareTo(right.Int));
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI32(left.I8.CompareTo(right.I8));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromI32(left.Byte.CompareTo(right.Byte));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI32(left.I16.CompareTo(right.I16));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromI32(left.U16.CompareTo(right.U16));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(left.I32.CompareTo(right.I32));
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromI32(left.U32.CompareTo(right.U32));
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI32(left.I64.CompareTo(right.I64));
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromI32(left.U64.CompareTo(right.U64));
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromI32(left.Offset.CompareTo(right.Offset));
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromI32(left.Size.CompareTo(right.Size));
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromI32(left.NInt.CompareTo(right.NInt));
        if (ReferenceEquals(plainType, PlainType.NUInt)) return Value.FromI32(left.NUInt.CompareTo(right.NUInt));
        if (plainType is BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor })
            return Value.FromI32(left.Int.CompareTo(right.Int));
        throw new NotImplementedException($"Compare `{type.ToILString()}`.");
    }

    [Inline]
    private static Result Not(Result result)
        => result.ShouldExit(out var value) ? result : Value.From(!value.Bool);

    private async ValueTask<Result> NegateAsync(
        BareType? selfBareType,
        IExpressionNode expression,
        LocalVariables variables)
    {
        var result = await ExecuteAsync(selfBareType, expression, variables).ConfigureAwait(false);
        if (result.ShouldExit(out var value)) return result;
        var type = expression.Type;
        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.From(-value.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)-value.I8);
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)-value.I16);
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(-value.I32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(-value.I64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(-value.Offset);
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromNInt(-value.NInt);
        if (type is CapabilityType { TypeConstructor: IntegerLiteralTypeConstructor }) return Value.From(-value.Int);
        throw new NotImplementedException($"Negate {type.ToILString()}");
    }

    [Inline]
    private static Value IdentityHash(Value value)
        => Value.FromNUInt((nuint)value.ClassReference.IdentityHash());

    private static Value Remainder(
        Value dividend,
        Value divisor,
        BareType selfType)
    {
        var plainType = selfType.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int)) return Value.From(dividend.Int % divisor.Int);
        if (ReferenceEquals(plainType, PlainType.UInt)) return Value.From(dividend.Int % divisor.Int);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)(dividend.I8 % divisor.I8));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromByte((byte)(dividend.Byte % divisor.Byte));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)(dividend.I16 % divisor.I16));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromU16((ushort)(dividend.U16 % divisor.U16));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(dividend.I32 % divisor.I32);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromU32(dividend.U32 % divisor.U32);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(dividend.I64 % divisor.I64);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromU64(dividend.U64 % divisor.U64);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(dividend.Offset % divisor.Offset);
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromSize(dividend.Size % divisor.Size);
        if (ReferenceEquals(plainType, PlainType.NInt)) return Value.FromOffset(dividend.NInt % divisor.NInt);
        if (ReferenceEquals(plainType, PlainType.NUInt)) return Value.FromSize(dividend.NUInt % divisor.NUInt);
        throw new NotImplementedException($"Remainder {selfType.ToILString()}");
    }

    private async ValueTask<Value> ToDisplayStringAsync(Value value, BareType selfType)
    {
        var plainType = selfType.PlainType;
        string displayString;
        if (ReferenceEquals(plainType, PlainType.Int)) displayString = value.Int.ToString();
        else if (ReferenceEquals(plainType, PlainType.UInt)) displayString = value.Int.ToString();
        else if (ReferenceEquals(plainType, PlainType.Byte)) displayString = value.Byte.ToString();
        else if (ReferenceEquals(plainType, PlainType.Int32)) displayString = value.I32.ToString();
        else if (ReferenceEquals(plainType, PlainType.UInt32)) displayString = value.U32.ToString();
        else if (ReferenceEquals(plainType, PlainType.Offset)) displayString = value.Offset.ToString();
        else if (ReferenceEquals(plainType, PlainType.Size)) displayString = value.Size.ToString();
        else if (ReferenceEquals(plainType, PlainType.NInt)) displayString = value.NInt.ToString();
        else if (ReferenceEquals(plainType, PlainType.NUInt)) displayString = value.NUInt.ToString();
        else throw new NotImplementedException($"to_display_string({selfType.ToILString()})");

        return await InitializeStringAsync(displayString).ConfigureAwait(false);
    }

    private async ValueTask<Result> ExecuteBlockOrResultAsync(
        BareType? selfBareType,
        IBlockOrResultNode statement,
        LocalVariables variables)
        => statement switch
        {
            IBlockExpressionNode b => await ExecuteAsync(selfBareType, b, variables).ConfigureAwait(false),
            IResultStatementNode s => await ExecuteAsync(selfBareType, s, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(statement)
        };

    private async ValueTask<Result> ExecuteElseAsync(
        BareType? selfBareType,
        IElseClauseNode elseClause,
        LocalVariables variables)
    {
        return elseClause switch
        {
            IBlockOrResultNode exp => await ExecuteBlockOrResultAsync(selfBareType, exp, variables).ConfigureAwait(false),
            IIfExpressionNode exp => await ExecuteAsync(selfBareType, exp, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(elseClause)
        };
    }

    private async ValueTask<Result> ExecuteAssignmentAsync(
        BareType? selfBareType,
        IExpressionNode expression,
        Value value,
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
                var result = await ExecuteAsync(selfBareType, exp.Context, variables).ConfigureAwait(false);
                if (result.ShouldExit(out var obj)) return result;
                // TODO handle the access operator
                obj.InstanceReference[exp.ReferencedDeclaration] = value;
                break;
        }

        return value;
    }

    public Task WaitForExitAsync() => executionTask;

    public TextReader StandardOutput { get; }
    public TextReader StandardError => TextReader.Null;

    public byte ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
}
