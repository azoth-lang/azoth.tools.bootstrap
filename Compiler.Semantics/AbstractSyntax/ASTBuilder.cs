using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Attribute = Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree.Attribute;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax;

// ReSharper disable once UnusedMember.Global
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes",
    Justification = "In Progress")]
internal class ASTBuilder
{
    // ReSharper disable once UnusedMember.Global
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public PackageBuilder BuildPackage(PackageSyntax<Package> packageSyntax)
    {
        var declarations = BuildNonMemberDeclarations(packageSyntax.EntityDeclarations, FixedSet.Empty<INonMemberDeclaration>());
        var testingDeclarations = BuildNonMemberDeclarations(packageSyntax.TestingEntityDeclarations, declarations);

        var symbolTree = packageSyntax.SymbolTree.Build();
        var testingSymbolTree = packageSyntax.TestingSymbolTree.Build();
        return new PackageBuilder(declarations, testingDeclarations, symbolTree, testingSymbolTree,
            packageSyntax.Diagnostics, packageSyntax.References.Select(r => r.Package).ToFixedSet());
    }

    private static IFixedSet<INonMemberDeclaration> BuildNonMemberDeclarations(
        IFixedSet<IEntityDefinitionSyntax> entityDeclarations,
        IFixedSet<INonMemberDeclaration> existingDeclarations)
    {
        var declarationDictionary = existingDeclarations.ToDictionary(d => d.Symbol, d => new Lazy<INonMemberDeclaration>(d));
        foreach (var declaration in entityDeclarations.OfType<INonMemberEntityDefinitionSyntax>())
        {
            declarationDictionary.Add(declaration.Symbol.Result,
                new(() => BuildNonMemberDeclaration(declaration, declarationDictionary)));
        }

        var declarations = declarationDictionary.Values.Select(l => l.Value).Except(existingDeclarations).ToFixedSet();
        return declarations;
    }

    private static INonMemberDeclaration BuildNonMemberDeclaration(
        INonMemberEntityDefinitionSyntax entity,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        return entity switch
        {
            IClassDefinitionSyntax syn => BuildClass(syn, declarations),
            IStructDefinitionSyntax syn => BuildStruct(syn, declarations),
            ITraitDefinitionSyntax syn => BuildTrait(syn, declarations),
            IFunctionDefinitionSyntax syn => BuildFunction(syn),
            _ => throw ExhaustiveMatch.Failed(entity)
        };
    }

    private static IClassDeclaration BuildClass(
        IClassDefinitionSyntax syn,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var baseClassSymbol = syn.BaseTypeName?.ReferencedSymbol.Result;
        var baseClass = baseClassSymbol is not null ? (IClassDeclaration)declarations[baseClassSymbol].Value : null;
        var supertypes = syn.SupertypeNames.Select(n => (ITypeDeclaration)declarations[n.ReferencedSymbol.Result!].Value).ToFixedList();
        var defaultConstructorSymbol = syn.DefaultConstructorSymbol;
        return new ClassDeclaration(syn.File, syn.Span, symbol, nameSpan, baseClass, supertypes, defaultConstructorSymbol, BuildMembers);

        IFixedList<IClassMemberDeclaration> BuildMembers(IClassDeclaration c)
            => syn.Members.Select(m => BuildClassMember(c, m)).ToFixedList();
    }

    private static IStructDeclaration BuildStruct(
        IStructDefinitionSyntax syn,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var supertypes = syn.SupertypeNames
                            .Select(n => (ITypeDeclaration)declarations[n.ReferencedSymbol.Result!].Value)
                            .ToFixedList();
        var defaultInitializerSymbol = syn.DefaultInitializerSymbol;
        return new StructDeclaration(syn.File, syn.Span, symbol, nameSpan, supertypes,
            defaultInitializerSymbol, BuildMembers);

        IFixedList<IStructMemberDeclaration> BuildMembers(IStructDeclaration s)
            => syn.Members.Select(m => BuildStructMember(s, m)).ToFixedList();
    }

    private static ITraitDeclaration BuildTrait(
        ITraitDefinitionSyntax syn,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var supertypes = syn.SupertypeNames.Select(n => (ITypeDeclaration)declarations[n.ReferencedSymbol.Result!].Value).ToFixedList();
        return new TraitDeclaration(syn.File, syn.Span, symbol, nameSpan, supertypes, BuildMembers);

        IFixedList<ITraitMemberDeclaration> BuildMembers(ITraitDeclaration t)
            => syn.Members.Select(m => BuildTraitMember(t, m)).ToFixedList();
    }

    private static IClassMemberDeclaration BuildClassMember(
        IClassDeclaration declaringClass,
        IClassMemberDefinitionSyntax member)
    {
        return member switch
        {
            IAssociatedFunctionDefinitionSyntax syn => BuildAssociatedFunction(declaringClass, syn),
            IAbstractMethodDefinitionSyntax syn => BuildAbstractMethod(declaringClass, syn),
            IStandardMethodDefinitionSyntax syn => BuildStandardMethod(declaringClass, syn),
            IGetterMethodDefinitionSyntax syn => BuildGetter(declaringClass, syn),
            ISetterMethodDefinitionSyntax syn => BuildSetter(declaringClass, syn),
            IConstructorDefinitionSyntax syn => BuildConstructor(declaringClass, syn),
            IFieldDefinitionSyntax syn => BuildField(declaringClass, syn),
            ITypeDefinitionSyntax _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(member)
        };
    }

    private static IStructMemberDeclaration BuildStructMember(
        IStructDeclaration declaringStruct,
        IStructMemberDefinitionSyntax member)
    {
        return member switch
        {
            IAssociatedFunctionDefinitionSyntax syn => BuildAssociatedFunction(declaringStruct, syn),
            IStandardMethodDefinitionSyntax syn => BuildStandardMethod(declaringStruct, syn),
            IGetterMethodDefinitionSyntax syn => BuildGetter(declaringStruct, syn),
            ISetterMethodDefinitionSyntax syn => BuildSetter(declaringStruct, syn),
            IInitializerDefinitionSyntax syn => BuildInitializer(declaringStruct, syn),
            IFieldDefinitionSyntax syn => BuildField(declaringStruct, syn),
            ITypeDefinitionSyntax _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(member)
        };
    }

    private static ITraitMemberDeclaration BuildTraitMember(
        ITypeDeclaration declaringTrait,
        ITraitMemberDefinitionSyntax member)
    {
        return member switch
        {
            IAssociatedFunctionDefinitionSyntax syn => BuildAssociatedFunction(declaringTrait, syn),
            IAbstractMethodDefinitionSyntax syn => BuildAbstractMethod(declaringTrait, syn),
            IStandardMethodDefinitionSyntax syn => BuildStandardMethod(declaringTrait, syn),
            IGetterMethodDefinitionSyntax syn => BuildGetter(declaringTrait, syn),
            ISetterMethodDefinitionSyntax syn => BuildSetter(declaringTrait, syn),
            ITypeDefinitionSyntax _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(member)
        };
    }

    private static IAssociatedFunctionDeclaration BuildAssociatedFunction(
        ITypeDeclaration declaringType,
        IAssociatedFunctionDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new AssociatedFunctionDeclaration(declaringType, syn.File, syn.Span, symbol, nameSpan, parameters, body);
    }

    private static IAbstractMethodDeclaration BuildAbstractMethod(
        ITypeDeclaration declaringType,
        IAbstractMethodDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        return new AbstractMethodDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan, selfParameter, parameters);
    }

    private static IStandardMethodDeclaration BuildStandardMethod(
        ITypeDeclaration declaringType,
        IStandardMethodDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new StandardMethodDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan, selfParameter, parameters, body);
    }

    private static IGetterMethodDeclaration BuildGetter(
        ITypeDeclaration declaringType,
        IGetterMethodDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var body = BuildBody(syn.Body);
        return new GetterMethodDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan,
            selfParameter, body);
    }

    private static ISetterMethodDeclaration BuildSetter(
        ITypeDeclaration declaringType,
        ISetterMethodDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new SetterMethodDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan,
            selfParameter, parameters, body);
    }

    private static IConstructorDeclaration BuildConstructor(
        IClassDeclaration declaringClass,
        IConstructorDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBlockBody(syn.Body);
        return new ConstructorDeclaration(syn.File, syn.Span, declaringClass, symbol, nameSpan, selfParameter, parameters, body);
    }

    private static IInitializerDeclaration BuildInitializer(
        IStructDeclaration declaringStruct,
        IInitializerDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBlockBody(syn.Body);
        return new InitializerDeclaration(syn.File, syn.Span, declaringStruct, symbol, nameSpan, selfParameter,
            parameters, body);
    }

    private static IFieldDeclaration BuildField(
        IClassOrStructDeclaration declaringType,
        IFieldDefinitionSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        return new FieldDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan);
    }

    private static IFunctionDeclaration BuildFunction(IFunctionDefinitionSyntax syn)
    {
        var attributes = BuildAttributes(syn.Attributes);
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new FunctionDeclaration(syn.File, syn.Span, attributes, symbol, nameSpan, parameters, body);
    }

    private static IFixedList<IAttribute> BuildAttributes(IEnumerable<IAttributeSyntax> syn)
        => syn.Select(BuildAttribute).ToFixedList();

    private static IAttribute BuildAttribute(IAttributeSyntax syn)
    {
        var referencedSymbol = syn.TypeName.ReferencedSymbol.Result.Assigned();
        return new Attribute(syn.Span, referencedSymbol);
    }

    private static IConstructorOrInitializerParameter BuildParameter(IConstructorOrInitializerParameterSyntax parameter)
    {
        return parameter switch
        {
            INamedParameterSyntax syn => BuildParameter(syn),
            IFieldParameterSyntax syn => BuildParameter(syn),
            _ => throw ExhaustiveMatch.Failed(parameter),
        };
    }

    private static INamedParameter BuildParameter(INamedParameterSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var defaultValue = BuildExpression(syn.DefaultValue);
        return new NamedParameter(syn.Span, symbol, syn.Unused, defaultValue);
    }

    private static ISelfParameter BuildParameter(ISelfParameterSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var unused = syn.Unused;
        return new SelfParameter(syn.Span, symbol, unused);
    }

    private static IFieldParameter BuildParameter(IFieldParameterSyntax syn)
    {
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var defaultValue = BuildExpression(syn.DefaultValue);
        return new FieldParameter(syn.Span, referencedSymbol, defaultValue);
    }

    private static IBody BuildBody(IBodySyntax syn)
    {
        return syn switch
        {
            IBlockBodySyntax body => BuildBlockBody(body),
            IExpressionBodySyntax body => BuildExpressionBody(body),
            _ => throw ExhaustiveMatch.Failed(syn),
        };
    }

    private static IBody BuildBlockBody(IBlockBodySyntax syn)
    {
        var statements = syn.Statements.Select(BuildBodyStatement).ToFixedList();
        return new Body(syn.Span, statements);
    }

    private static IBody BuildExpressionBody(IExpressionBodySyntax syn)
    {
        var exp = BuildExpression(syn.ResultStatement.Expression);
        var returnExp = new ReturnExpression(syn.ResultStatement.Span, exp.DataType, exp);
        var returnStmt = new ExpressionStatement(syn.ResultStatement.Span, returnExp);
        return new Body(syn.Span, returnStmt.Yield().ToFixedList<IBodyStatement>());
    }

    private static IStatement BuildStatement(IStatementSyntax stmt)
    {
        return stmt switch
        {
            IResultStatementSyntax syn => BuildResultStatement(syn),
            IBodyStatementSyntax syn => BuildBodyStatement(syn),
            _ => throw ExhaustiveMatch.Failed(stmt),
        };
    }

    private static IBodyStatement BuildBodyStatement(IBodyStatementSyntax stmt)
    {
        return stmt switch
        {
            IExpressionStatementSyntax syn => BuildExpressionStatement(syn),
            IVariableDeclarationStatementSyntax syn => BuildVariableDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(stmt),
        };
    }

    private static IExpressionStatement BuildExpressionStatement(IExpressionStatementSyntax syn)
    {
        var expression = BuildExpression(syn.Expression);
        return new ExpressionStatement(syn.Span, expression);
    }

    private static IVariableDeclarationStatement BuildVariableDeclaration(IVariableDeclarationStatementSyntax syn)
    {
        var nameSpan = syn.NameSpan;
        var symbol = syn.Symbol.Result;
        var initializer = BuildExpression(syn.Initializer);
        return new VariableDeclarationStatement(syn.Span, nameSpan, symbol, initializer);
    }

    private static IResultStatement BuildResultStatement(IResultStatementSyntax syn)
    {
        var expression = BuildExpression(syn.Expression);
        return new ResultStatement(syn.Span, expression);
    }

    private static IBlockOrResult BuildBlockOrResult(IBlockOrResultSyntax syntax)
    {
        return syntax switch
        {
            IBlockExpressionSyntax syn => BuildBlockExpression(syn),
            IResultStatementSyntax syn => BuildResultStatement(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };
    }

    [return: NotNullIfNotNull(nameof(expressionSyntax))]
    private static IExpression? BuildExpression(IExpressionSyntax? expressionSyntax, bool isMove = false)
    {
        if (expressionSyntax is null) return null;
        IExpression expression = expressionSyntax switch
        {
            IAssignmentExpressionSyntax syn => BuildAssignmentExpression(syn),
            IBinaryOperatorExpressionSyntax syn => BuildBinaryOperatorExpression(syn),
            IBlockExpressionSyntax syn => BuildBlockExpression(syn),
            IBoolLiteralExpressionSyntax syn => BuildBoolLiteralExpression(syn),
            IBreakExpressionSyntax syn => BuildBreakExpression(syn),
            IMemberAccessExpressionSyntax syn => BuildGetterAccessExpression(syn),
            IForeachExpressionSyntax syn => BuildForeachExpression(syn),
            IIfExpressionSyntax syn => BuildIfExpression(syn),
            IIntegerLiteralExpressionSyntax syn => BuildIntegerLiteralExpression(syn),
            INoneLiteralExpressionSyntax syn => BuildNoneLiteralExpression(syn),
            IStringLiteralExpressionSyntax syn => BuildStringLiteralExpression(syn),
            ILoopExpressionSyntax syn => BuildLoopExpression(syn),
            IMoveExpressionSyntax syn => BuildMoveExpression(syn),
            IIdentifierNameExpressionSyntax syn => BuildNameExpression(syn, isMove),
            ISpecialTypeNameExpressionSyntax syn => BuildNameExpression(syn),
            IGenericNameExpressionSyntax syn => throw new NotImplementedException(),
            IMissingNameSyntax syn => throw new NotSupportedException(),
            INewObjectExpressionSyntax syn => BuildNewObjectExpression(syn),
            IInvocationExpressionSyntax syn => BuildInvocationExpression(syn),
            INextExpressionSyntax syn => BuildNextExpression(syn),
            IReturnExpressionSyntax syn => BuildReturnExpression(syn),
            ISelfExpressionSyntax syn => BuildSelfExpression(syn),
            IUnaryOperatorExpressionSyntax syn => BuildUnaryOperatorExpression(syn),
            IUnsafeExpressionSyntax syn => BuildUnsafeExpression(syn),
            IWhileExpressionSyntax syn => BuildWhileExpression(syn),
            IIdExpressionSyntax syn => BuildIdExpression(syn),
            IFreezeExpressionSyntax syn => BuildFreezeExpression(syn),
            IConversionExpressionSyntax syn => BuildConversionExpression(syn),
            IPatternMatchExpressionSyntax syn => BuildPatternMatchExpression(syn),
            IAsyncBlockExpressionSyntax syn => BuildAsyncBlockExpression(syn),
            IAsyncStartExpressionSyntax syn => BuildAsyncStartExpression(syn),
            IAwaitExpressionSyntax syn => BuildAwaitExpression(syn),
            _ => throw ExhaustiveMatch.Failed(expressionSyntax),
        };
        return BuildImplicitConversion(expression, expressionSyntax);
    }

    private static IExpression BuildImplicitConversion(IExpression expression, IExpressionSyntax expressionSyntax)
    {
        if (expressionSyntax.ImplicitConversion is Conversion conversion)
            return BuildImplicitConversion(expression, conversion);

        return expression;
    }

    private static IExpression BuildImplicitConversion(IExpression expression, Conversion conversion)
    {
        if (conversion is ChainedConversion chainedConversion)
            expression = BuildImplicitConversion(expression, chainedConversion.PriorConversion);
        return conversion switch
        {
            LiftedConversion _ => BuildImplicitLiftedConversionExpression(expression, conversion),
            SimpleTypeConversion c => BuildImplicitSimpleTypeConversionExpression(expression, c),
            OptionalConversion _ => BuildImplicitOptionalConversionExpression(expression, conversion),
            MoveConversion moveConversion => BuildMoveConversionExpression(expression, moveConversion),
            FreezeConversion freezeConversion => BuildFreezeConversionExpression(expression, freezeConversion),
            IdentityConversion _ => expression,
            _ => throw ExhaustiveMatch.Failed(conversion)
        };
    }

    private static IFixedList<IExpression> BuildExpressions(IFixedList<IExpressionSyntax> expressions)
        // The compiler isn't able to correctly figure out the nullability here. That is actually
        // why this method even exists
        => expressions.Select(syn => BuildExpression(syn)).ToFixedList()!;

    private static IExpression BuildAssignmentExpression(IAssignmentExpressionSyntax syn)
    {
        var leftOperandSymbol = syn.LeftOperand.ReferencedSymbol.Result.Assigned();
        switch (leftOperandSymbol)
        {
            case BindingSymbol _:
            {
                var type = syn.DataType.Assigned();
                var leftOperand = BuildAssignableExpression(syn.LeftOperand);
                var @operator = syn.Operator;
                var rightOperand = BuildExpression(syn.RightOperand);
                return new AssignmentExpression(syn.Span, type, leftOperand, @operator, rightOperand);
            }
            case MethodSymbol methodSymbol:
            {
                var type = syn.DataType.Assigned();
                var contextSyntax = ((IMemberAccessExpressionSyntax)syn.LeftOperand).Context;
                var context = BuildExpression(contextSyntax);
                var rightOperand = BuildExpression(syn.RightOperand);
                return new MethodInvocationExpression(syn.Span, type, context,
                    methodSymbol, FixedList.Create(rightOperand));
            }
            default:
                throw new NotSupportedException();
        }
    }

    private static IAssignableExpression BuildAssignableExpression(IAssignableExpressionSyntax expression)
    {
        return expression switch
        {
            IMemberAccessExpressionSyntax syn => BuildFieldAccessExpression(syn),
            IIdentifierNameExpressionSyntax syn => BuildVariableNameExpression(syn, false),
            IMissingNameSyntax syn => throw new NotSupportedException(),
            _ => throw ExhaustiveMatch.Failed(expression),
        };
    }

    private static IBinaryOperatorExpression BuildBinaryOperatorExpression(IBinaryOperatorExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var leftOperand = BuildExpression(syn.LeftOperand);
        var @operator = syn.Operator;
        var rightOperand = BuildExpression(syn.RightOperand);
        return new BinaryOperatorExpression(syn.Span, type, leftOperand, @operator, rightOperand);
    }

    private static IBlockExpression BuildBlockExpression(IBlockExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var statements = syn.Statements.Select(BuildStatement).ToFixedList();
        return new BlockExpression(syn.Span, type, statements);
    }

    private static IBoolLiteralExpression BuildBoolLiteralExpression(IBoolLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var value = syn.Value;
        return new BoolLiteralExpression(syn.Span, type, value);
    }

    private static IBreakExpression BuildBreakExpression(IBreakExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var value = BuildExpression(syn.Value);
        return new BreakExpression(syn.Span, type, value);
    }

    private static IExpression BuildGetterAccessExpression(IMemberAccessExpressionSyntax syn)
    {
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        switch (referencedSymbol)
        {
            case FieldSymbol _:
                return BuildFieldAccessExpression(syn);
            case MethodSymbol methodSymbol:
            {
                var type = syn.DataType.Assigned();
                var context = BuildExpression(syn.Context);
                return new MethodInvocationExpression(syn.Span, type, context, methodSymbol,
                    FixedList.Empty<IExpression>());
            }
            default:
                throw new NotSupportedException();
        }
    }

    private static IFieldAccessExpression BuildFieldAccessExpression(IMemberAccessExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var context = BuildExpression(syn.Context);
        var fieldSymbol = (FieldSymbol)syn.ReferencedSymbol.Result.Assigned();
        return new FieldAccessExpression(syn.Span, type, context, fieldSymbol);
    }

    private static IForeachExpression BuildForeachExpression(IForeachExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var symbol = syn.Symbol.Result;
        var inExpression = BuildExpression(syn.InExpression);
        var iterateMethod = syn.IterateMethod.Result;
        var nextMethod = syn.NextMethod.Result;
        var block = BuildBlockExpression(syn.Block);
        return new ForeachExpression(syn.Span, type, symbol, inExpression, iterateMethod, nextMethod, block);
    }

    private static IInvocationExpression BuildInvocationExpression(IInvocationExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result;
        var arguments = BuildExpressions(syn.Arguments);
        switch (referencedSymbol)
        {
            default:
                throw ExhaustiveMatch.Failed(referencedSymbol);
            case null:
                var referent = BuildExpression(syn.Expression);
                return new FunctionReferenceInvocationExpression(syn.Span, type, referent, arguments);
            case FunctionSymbol function:
                return new FunctionInvocationExpression(syn.Span, type, function, arguments);
            case MethodSymbol method:
                var qualifiedName = (IMemberAccessExpressionSyntax)syn.Expression;
                var context = BuildExpression(qualifiedName.Context);
                return new MethodInvocationExpression(syn.Span, type, context, method, arguments);
            case InitializerSymbol initializer:
                return new InitializerInvocationExpression(syn.Span, type, initializer, arguments);
            case BindingSymbol _:
                var field = BuildExpression(syn.Expression);
                return new FunctionReferenceInvocationExpression(syn.Span, type, field, arguments);
            case TypeSymbol _:
                throw new InvalidOperationException("Invocation expression cannot invoke a type.");
            case ConstructorSymbol _:
                throw new InvalidOperationException("Invocation expression cannot invoke a constructor.");
            case NamespaceSymbol _:
                throw new InvalidOperationException("Invocation expression cannot invoke a namespace or package.");
        }
    }

    private static IIfExpression BuildIfExpression(IIfExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var condition = BuildExpression(syn.Condition);
        var thenBlock = BuildBlockOrResult(syn.ThenBlock);
        var elseClause = BuildElseClause(syn.ElseClause);
        return new IfExpression(syn.Span, type, condition, thenBlock, elseClause);
    }

    [return: NotNullIfNotNull(nameof(elseClause))]
    private static IElseClause? BuildElseClause(IElseClauseSyntax? elseClause)
    {
        return elseClause switch
        {
            null => null,
            IBlockOrResultSyntax syn => BuildBlockOrResult(syn),
            IIfExpressionSyntax syn => BuildIfExpression(syn),
            _ => throw ExhaustiveMatch.Failed(elseClause),
        };
    }

    private static IImplicitLiftedConversionExpression BuildImplicitLiftedConversionExpression(
        IExpression expression,
        Conversion conversion)
    {
        var convertToType = conversion.Apply(expression.DataType);
        return new ImplicitLiftedConversion(expression.Span, convertToType, expression, (OptionalType)convertToType);
    }

    private static IImplicitSimpleTypeConversionExpression BuildImplicitSimpleTypeConversionExpression(
        IExpression expression,
        SimpleTypeConversion conversion)
        => new ImplicitSimpleTypeConversionExpression(expression.Span, expression, conversion.To);

    private static IImplicitOptionalConversionExpression BuildImplicitOptionalConversionExpression(
        IExpression expression,
        Conversion conversion)
    {
        var convertToType = conversion.Apply(expression.DataType);
        return new ImplicitOptionalConversionExpression(expression.Span, convertToType, expression, (OptionalType)convertToType);
    }

    private static IExpression BuildMoveConversionExpression(
        IExpression expression,
        MoveConversion conversion)
    {
        var convertToType = conversion.Apply(expression.DataType);
        switch (conversion.Kind)
        {
            case ConversionKind.Recover:
                return new RecoverIsolationExpression(expression.Span, (CapabilityType)convertToType, expression);
            case ConversionKind.Implicit:
                var variableNameExpression = (IVariableNameExpression)expression;
                var referencedSymbol = variableNameExpression.ReferencedSymbol;
                return new MoveExpression(expression.Span, (CapabilityType)convertToType, referencedSymbol,
                    variableNameExpression);
            case ConversionKind.Temporary:
                return new TempMoveExpression(expression.Span, (CapabilityType)convertToType, expression);
            default:
                throw ExhaustiveMatch.Failed(conversion.Kind);
        }
    }

    private static IExpression BuildFreezeConversionExpression(
        IExpression expression,
        FreezeConversion conversion)
    {
        var convertToType = conversion.Apply(expression.DataType);
        switch (conversion.Kind)
        {
            case ConversionKind.Recover:
                return new RecoverConstExpression(expression.Span, (CapabilityType)convertToType, expression);
            case ConversionKind.Implicit:
                var variableNameExpression = (IVariableNameExpression)expression;
                var referencedSymbol = ((IVariableNameExpression)expression).ReferencedSymbol;
                return new FreezeExpression(expression.Span, (CapabilityType)convertToType, referencedSymbol,
                    variableNameExpression);
            case ConversionKind.Temporary:
                return new TempFreezeExpression(expression.Span, (CapabilityType)convertToType, expression);
            default:
                throw ExhaustiveMatch.Failed(conversion.Kind);
        }
    }

    private static IIntegerLiteralExpression BuildIntegerLiteralExpression(IIntegerLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var value = syn.Value;
        return new IntegerLiteralExpression(syn.Span, type, value);
    }

    private static INoneLiteralExpression BuildNoneLiteralExpression(INoneLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        return new NoneLiteralExpression(syn.Span, type);
    }

    private static IStringLiteralExpression BuildStringLiteralExpression(IStringLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var value = syn.Value;
        return new StringLiteralExpression(syn.Span, type, value);
    }

    private static ILoopExpression BuildLoopExpression(ILoopExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var block = BuildBlockExpression(syn.Block);
        return new LoopExpression(syn.Span, type, block);
    }

    private static IMoveExpression BuildMoveExpression(IMoveExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var referent = BuildExpression(syn.Referent, true);
        return new MoveExpression(syn.Span, type, referencedSymbol, referent);
    }

    private static INameExpression BuildNameExpression(ISpecialTypeNameExpressionSyntax _)
        => throw new InvalidOperationException("Cannot build a name expression for a type.");

    private static INameExpression BuildNameExpression(IIdentifierNameExpressionSyntax syn, bool isMove)
    {
        var symbol = ((IStandardNameExpressionSyntax)syn).ReferencedSymbol.Result.Assigned();
        return symbol switch
        {
            NamedVariableSymbol sym => BuildVariableNameExpression(syn, sym, isMove),
            FunctionSymbol sym => BuildFunctionNameExpression(syn, sym),
            TypeSymbol _ => throw new InvalidOperationException("Cannot build a name expression for a type."),
            InvocableSymbol _ => throw new InvalidOperationException("Cannot build a name expression for an invocable."),
            NamespaceSymbol _ => throw new InvalidOperationException("Cannot build a name expression for a namespace or package."),
            FieldSymbol _ => throw new UnreachableException("Field would be a different expression."),
            SelfParameterSymbol _ => throw new UnreachableException("Self parameter would be a different expression."),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };
    }

    private static IVariableNameExpression BuildVariableNameExpression(IIdentifierNameExpressionSyntax syn, bool isMove)
    {
        var variableSymbol = (NamedVariableSymbol)((IStandardNameExpressionSyntax)syn).ReferencedSymbol.Result.Assigned();
        return BuildVariableNameExpression(syn, variableSymbol, isMove);
    }

    private static IVariableNameExpression BuildVariableNameExpression(IIdentifierNameExpressionSyntax syn, NamedVariableSymbol referencedSymbol, bool isMove)
    {
        var type = syn.DataType.Assigned();
        return new VariableNameExpression(syn.Span, type, referencedSymbol, isMove);
    }

    private static IFunctionNameExpression BuildFunctionNameExpression(IIdentifierNameExpressionSyntax syn, FunctionSymbol referencedSymbol)
    {
        var type = syn.DataType.Assigned();
        return new FunctionNameExpression(syn.Span, type, referencedSymbol);
    }

    private static INewObjectExpression BuildNewObjectExpression(INewObjectExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var arguments = BuildExpressions(syn.Arguments);
        return new NewObjectExpression(syn.Span, type, referencedSymbol, arguments);
    }

    private static INextExpression BuildNextExpression(INextExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        return new NextExpression(syn.Span, type);
    }

    private static IReturnExpression BuildReturnExpression(IReturnExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var value = BuildExpression(syn.Value);
        return new ReturnExpression(syn.Span, type, value);
    }

    private static ISelfExpression BuildSelfExpression(ISelfExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var isImplicit = syn.IsImplicit;
        return new SelfExpression(syn.Span, type, referencedSymbol, isImplicit);
    }

    private static IUnaryOperatorExpression BuildUnaryOperatorExpression(IUnaryOperatorExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var fixity = syn.Fixity;
        var @operator = syn.Operator;
        var operand = BuildExpression(syn.Operand);
        return new UnaryOperatorExpression(syn.Span, type, fixity, @operator, operand);
    }

    private static IUnsafeExpression BuildUnsafeExpression(IUnsafeExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new UnsafeExpression(syn.Span, type, expression);
    }

    private static IWhileExpression BuildWhileExpression(IWhileExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var condition = BuildExpression(syn.Condition);
        var block = BuildBlockExpression(syn.Block);
        return new WhileExpression(syn.Span, type, condition, block);
    }

    private static IExpression BuildIdExpression(IIdExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var referent = BuildExpression(syn.Referent);
        return new IdExpression(syn.Span, type, referent);
    }

    private static IFreezeExpression BuildFreezeExpression(IFreezeExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var referent = BuildExpression(syn.Referent);
        return new FreezeExpression(syn.Span, type, referencedSymbol, referent);
    }

    private static IExpression BuildConversionExpression(IConversionExpressionSyntax syn)
    {
        var referent = BuildExpression(syn.Referent);

        // TODO support non-numeric conversions
        var convertToType = (ValueType)syn.ConvertToType.NamedType.Assigned();
        var convertToSimpleType = (SimpleType)convertToType.DeclaredType;

        if (syn.Operator == ConversionOperator.Safe)
            return new ImplicitSimpleTypeConversionExpression(syn.Span, referent, convertToSimpleType);

        var isOptional = syn.Operator == ConversionOperator.Optional;
        return new ExplicitSimpleTypeConversion(syn.Span, referent, isOptional, convertToSimpleType);
    }

    private static IExpression BuildPatternMatchExpression(IPatternMatchExpressionSyntax syn)
    {
        var referent = BuildExpression(syn.Referent);
        var pattern = BuildPattern(syn.Pattern);
        return new PatternMatchExpression(referent, pattern);
    }

    private static IPattern BuildPattern(IPatternSyntax patternSyntax)
    {
        return patternSyntax switch
        {
            IBindingContextPatternSyntax syn => BuildBindingContextPattern(syn),
            IBindingPatternSyntax syn => BuildBindingPattern(syn),
            IOptionalPatternSyntax syn => BuildOptionalPattern(syn),
            _ => throw ExhaustiveMatch.Failed(patternSyntax),
        };
    }

    private static IPattern BuildBindingContextPattern(IBindingContextPatternSyntax syn)
    {
        var pattern = BuildPattern(syn.Pattern);
        if (syn.Type is null) return pattern;
        var type = syn.Type.NamedType.Assigned();
        return new BindingContextPattern(syn.Span, pattern, type);
    }

    private static IBindingPattern BuildBindingPattern(IBindingPatternSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        return new BindingPattern(syn.Span, symbol);
    }

    private static IOptionalPattern BuildOptionalPattern(IOptionalPatternSyntax syn)
    {
        var pattern = BuildPattern(syn.Pattern);
        return new OptionalPattern(syn.Span, pattern);
    }

    private static IAsyncBlockExpression BuildAsyncBlockExpression(IAsyncBlockExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var block = BuildBlockExpression(syn.Block);
        return new AsyncBlockExpression(syn.Span, type, block);
    }

    private static IAsyncStartExpression BuildAsyncStartExpression(IAsyncStartExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new AsyncStartExpression(syn.Span, type, syn.Scheduled, expression);
    }

    private static IAwaitExpression BuildAwaitExpression(IAwaitExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new AwaitExpression(syn.Span, type, expression);
    }
}
