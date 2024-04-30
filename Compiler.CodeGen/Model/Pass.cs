using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types.Type;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class Pass
{
    public PassNode Syntax { get; }
    public string Namespace => Syntax.Namespace;
    public string Name => Syntax.Name;
    public IFixedSet<string> UsingNamespaces => Syntax.UsingNamespaces;
    public Symbol? FromContext { get; }
    public Symbol? ToContext { get; }
    public SymbolNode? From => Syntax.From;
    public Language? FromLanguage { get; }
    public SymbolNode? To => Syntax.To;
    public Language? ToLanguage { get; }
    [MemberNotNullWhen(true, nameof(ToLanguage))]
    public bool IsLanguageTransform { get; }

    private readonly Parameter? fromParameter;
    private readonly Parameter? toParameter;
    public Parameter? FromContextParameter { get; }
    public Parameter? ToContextParameter { get; }
    public IFixedList<Parameter> PassReturnValues { get; }
    public IFixedList<Transform> Transforms { get; }
    public IFixedList<Parameter> RunParameters { get; }
    public IFixedList<Parameter> RunReturnValues { get; }
    public IFixedList<Parameter> PassParameters { get; }

    public IFixedList<SimpleCreateMethod> SimpleCreateMethods { get; }
    public IFixedList<AdvancedCreateMethod> AdvancedCreateMethods { get; }
    public IFixedList<TransformMethod> TransformMethods { get; }
    public TransformMethod EntryTransformMethod { get; }
    public IFixedList<Method> Methods { get; }

    public Pass(PassNode syntax, LanguageLoader languageLoader)
    {
        Syntax = syntax;
        FromLanguage = GetOrLoadLanguageNamed(Syntax.From, languageLoader);
        ToLanguage = GetOrLoadLanguageNamed(Syntax.To, languageLoader);
        FromContext = Symbol.CreateExternal(Syntax.FromContext);
        ToContext = Symbol.CreateExternal(Syntax.ToContext);
        IsLanguageTransform = ToLanguage is not null && FromLanguage != ToLanguage;

        fromParameter = CreateFromParameter();
        FromContextParameter = CreateFromContextParameter();
        RunParameters = Parameters(fromParameter, FromContextParameter);
        PassParameters = Parameters(fromParameter ?? Parameter.Create(Type.VoidSymbol, "from"),
            FromContextParameter ?? Parameter.Create(Type.VoidSymbol, "context"));
        toParameter = CreateToParameter();
        ToContextParameter = CreateToContextParameter();
        RunReturnValues = Parameters(toParameter, ToContextParameter);
        PassReturnValues = Parameters(toParameter ?? Parameter.Create(Type.VoidSymbol, "to"),
            ToContextParameter ?? Parameter.Create(Type.VoidSymbol, "toContext"));
        Transforms = Syntax.Transforms.Select(t => new Transform(this, t)).ToFixedList();
        SimpleCreateMethods = CreateSimpleCreateMethods().ToFixedList();
        AdvancedCreateMethods = CreateAdvancedCreateMethods().ToFixedList();
        TransformMethods = CreateTransformMethods().ToFixedList();
        Methods = SimpleCreateMethods
                  .Concat<Method>(AdvancedCreateMethods)
                  .Concat(TransformMethods).ToFixedList();
        // Bubble and recreate methods with additional parameters
        Methods = BubbleMethodParameters();
        SimpleCreateMethods = Methods.OfType<SimpleCreateMethod>().ToFixedList();
        AdvancedCreateMethods = Methods.OfType<AdvancedCreateMethod>().ToFixedList();
        TransformMethods = Methods.OfType<TransformMethod>().ToFixedList();
        EntryTransformMethod = TransformMethods.Single(IsEntryTransform);
    }

    private static Language? GetOrLoadLanguageNamed(SymbolNode? name, LanguageLoader languageLoader)
        => name is not null && !name.IsQuoted ? languageLoader.GetOrLoadLanguageNamed(name.Text) : null;

    private Parameter? CreateFromParameter()
    {
        var fromType = SymbolType.Create(FromLanguage?.Entry ?? Symbol.CreateExternalFromSyntax(Syntax.From));
        return Parameter.Create(fromType, "from");
    }

    private Parameter? CreateFromContextParameter()
    {
        var contextType = SymbolType.Create(FromContext);
        var contextParameter = Parameter.Create(contextType, "context");
        return contextParameter;
    }

    private Parameter? CreateToParameter()
    {
        var toType = SymbolType.Create(ToLanguage?.Entry ?? Symbol.CreateExternalFromSyntax(Syntax.To));
        return Parameter.Create(toType, "to");
    }

    private Parameter? CreateToContextParameter()
    {
        var contextType = SymbolType.Create(ToContext);
        var contextParameter = Parameter.Create(contextType, "toContext");
        return contextParameter;
    }

    private static IFixedList<Parameter> Parameters(params Parameter?[] parameters)
        => parameters.WhereNotNull().ToFixedList();

    private static IEnumerable<Parameter> MergeByName(
        IEnumerable<Parameter> parentAdditionalParameters,
        IEnumerable<Parameter> missingAndModifiedParameters)
        => parentAdditionalParameters.Concat(missingAndModifiedParameters).MergeByName();

    private static bool IsChildRule(NonVoidType? fromType, NonVoidType? toType)
        => fromType?.UnderlyingSymbol is InternalSymbol { ReferencedRule: var rule }
           && toType?.UnderlyingSymbol is InternalSymbol { ReferencedRule: var parentRule }
           && (rule == parentRule || rule.AncestorRules.Contains(parentRule));

    private static IFixedList<Parameter> MissingAndModifiedParameters(
        IFixedList<Parameter> parentAdditionalParameters,
        IEnumerable<Parameter> additionalParameters)
    {
        var lookup = parentAdditionalParameters.ToLookup(p => p.Name);
        var missingParameters = additionalParameters
                                .Except(parentAdditionalParameters)
                                .Where(p => p.Type is not OptionalType optionalType
                                            || lookup[p.Name].All(pp => pp.Type != optionalType.UnderlyingType))
                                .ToFixedList();
        return missingParameters;
    }

    private IEnumerable<SimpleCreateMethod> CreateSimpleCreateMethods()
    {
        if (!IsLanguageTransform)
            yield break;

        foreach (var rule in ToLanguage.Grammar.Rules.Where(ShouldCreate))
            yield return new SimpleCreateMethod(this, rule);

        yield break;

        // If a rule is a nonterminal, we can't take parameters for all the derived rules properties
        // If a rule doesn't extend another rule, then there is no `from` parameter
        static bool ShouldCreate(Rule r) => r is { IsTerminal: true, ExtendsRule: not null };
    }

    private IEnumerable<AdvancedCreateMethod> CreateAdvancedCreateMethods()
    {
        if (!IsLanguageTransform)
            yield break;

        var toGrammar = ToLanguage.Grammar;
        foreach (var rule in toGrammar.Rules.Where(ShouldCreate))
            if (rule.IsTerminal)
                yield return new AdvancedCreateTerminalMethod(this, rule);
            else
                yield return new AdvancedCreateNonTerminalMethod(this, rule);

        yield break;

        bool ShouldCreate(Rule r)
           => r is { ExtendsRule: not null }
              && (r.DifferentChildProperties.Any() || (!r.IsTerminal && DerivedRulesShouldBeCreated(r)));

        bool DerivedRulesShouldBeCreated(Rule rule)
        {
            var fromRule = rule.ExtendsRule!;
            foreach (var derivedRule in fromRule.DerivedRules)
            {
                var derivedToRule = toGrammar.RuleFor(derivedRule.Defines);
                if (derivedToRule is null)
                    return false; // Not possible to transform derived rule
                if (derivedToRule.IsTerminal)
                    continue; // Terminals are covered by simple create methods
                if (!ShouldCreate(derivedToRule))
                    return false;
            }
            return true;
        }
    }

    private IEnumerable<TransformMethod> CreateTransformMethods()
    {
        var transforms = CreateDeclaredTransformMethods().ToDictionary(t => t.FromCoreType);

        if (!IsLanguageTransform || FromLanguage is null)
            return transforms.Values;

        var fromGrammar = FromLanguage.Grammar;
        var toGrammar = ToLanguage.Grammar;

        // Collect the values to avoid modifying the dictionary while iterating
        foreach (var baseTransform in CreateTransformsForModifiedTerminals(transforms, toGrammar).ToArray())
            transforms.Add(baseTransform.FromCoreType, baseTransform);

        var toExpand = new Queue<TransformMethod>(transforms.Values);

        while (toExpand.TryDequeue(out var transform))
        {
            foreach (var baseTransform in CreateBaseTransforms(transform, transforms, toGrammar))
            {
                transforms.Add(baseTransform.FromCoreType, baseTransform);
                toExpand.Enqueue(baseTransform);
            }

            foreach (var parentTransform in CreateParentTransforms(transform, transforms, fromGrammar, toGrammar))
            {
                transforms[parentTransform.FromCoreType] = parentTransform;
                toExpand.Enqueue(parentTransform);
            }

            foreach (var childTransform in CreateChildTransforms(transform, transforms, toGrammar))
            {
                transforms.Add(childTransform.FromCoreType, childTransform);
                toExpand.Enqueue(childTransform);
            }
        }

        return transforms.Values;
    }

    private IEnumerable<TransformMethod> CreateDeclaredTransformMethods()
    {
        foreach (var transform in Transforms)
        {
            var fromType = transform.From?.Type;
            var toType = transform.To?.Type;
            if (fromType is CollectionType fromCollectionType && toType is CollectionType toCollectionType)
                yield return new TransformCollectionMethod(this, transform, fromCollectionType, toCollectionType);
            else if (transform.AutoGenerate
                     && fromType is not null
                     && toType?.UnderlyingSymbol is InternalSymbol { ReferencedRule.DescendantsModified: false })
                yield return new TransformIdentityMethod(this, transform, fromType, toType);
            else if (fromType is not null)
            {
                var referencedRule = (fromType.UnderlyingSymbol as InternalSymbol)?.ReferencedRule;
                var isTerminal = referencedRule?.IsTerminal;
                TransformMethod declaredTransformMethods = isTerminal switch
                {
                    true or null => new TransformTerminalMethod(this, transform, fromType, toType),
                    false => new TransformNonTerminalMethod(this, transform, referencedRule!, fromType, toType),
                };
                yield return declaredTransformMethods;
            }
            else
                throw new NotImplementedException();
        }
    }

    private IEnumerable<TransformMethod> CreateTransformsForModifiedTerminals(
        IReadOnlyDictionary<NonOptionalType, TransformMethod> transforms,
        Grammar toGrammar)
    {
        // Add auto transforms for all rules that are modified terminals
        var modifiedRules = toGrammar.Rules.Where(r => r is { IsModified: true, IsTerminal: true });
        foreach (var toRule in modifiedRules)
        {
            var fromRule = toRule.ExtendsRule!;
            var fromType = new SymbolType(fromRule.Defines);
            if (transforms.ContainsKey(fromType)) continue;

            var transform = CreateTransformMethod(fromRule, transforms, toGrammar);
            if (transform is not null)
                yield return transform;
        }
    }

    private IEnumerable<TransformMethod> CreateBaseTransforms(
        TransformMethod transform,
        IReadOnlyDictionary<NonOptionalType, TransformMethod> transforms,
        Grammar toGrammar)
    {
        if (transform.FromCoreType is not SymbolType { Symbol: InternalSymbol symbol })
            yield break;

        foreach (var baseRule in symbol.ReferencedRule.BaseRules)
        {
            if (transforms.ContainsKey(baseRule.DefinesType))
                continue; // Transform already exists

            var parentTransform = CreateTransformMethod(baseRule, transforms, toGrammar);

            if (parentTransform is null) continue;

            yield return parentTransform;
        }
    }

    private IEnumerable<TransformMethod> CreateParentTransforms(
        TransformMethod transform,
        IReadOnlyDictionary<NonOptionalType, TransformMethod> transforms,
        Grammar fromGrammar,
        Grammar toGrammar)
    {
        var fromCoreType = transform.FromCoreType;
        foreach (var rule in fromGrammar.Rules)
        {
            // If this transform is not optional, it may need to be made optional
            if (transform is { ParametersDeclared: false, FromType: not OptionalType }
                && rule.AllProperties.Select(p => p.Type).OfType<OptionalType>()
                       .Any(t => t.UnderlyingType == transform.FromType))
            {
                // Replace the transform with an optional one
                yield return transform.ToOptional();

                // The transform will be visited again, so stop for now
                yield break;
            }

            // For transforms that are not already defined, define them if this type is a direct child of the rule
            if (!transforms.ContainsKey(rule.DefinesType)
                && rule.AllProperties.Any(p => p.Type == fromCoreType))
            {
                var parentTransform = CreateTransformMethod(rule, transforms, toGrammar);

                if (parentTransform is not null)
                    yield return parentTransform;
            }

            foreach (var type in rule.AllProperties
                                     .Select(p => p.Type).OfType<CollectionType>()
                                     .Where(c => c.ElementType == fromCoreType))
            {
                var parentTransform = CreateTransformMethod(type, toGrammar);
                if (parentTransform is not null)
                    yield return parentTransform;
            }
        }
    }

    private IEnumerable<TransformMethod> CreateChildTransforms(
        TransformMethod transform,
        IReadOnlyDictionary<NonOptionalType, TransformMethod> transforms,
        Grammar toGrammar)
    {
        var fromType = transform.FromType;
        if (fromType.UnderlyingSymbol is not InternalSymbol { ReferencedRule: { IsTerminal: false } rule })
            yield break;

        foreach (var derivedRule in rule.DerivedRules.Where(r => r.DescendantsModified))
        {
            var derivedFromType = derivedRule.DefinesType;
            if (transforms.ContainsKey(derivedFromType))
                continue;
            // Derived type isn't covered yet, add a transform for it
            var derivedTransform = CreateTransformMethod(derivedRule, transforms, toGrammar);
            if (derivedTransform is null)
                continue;

            yield return derivedTransform;
        }
    }

    private TransformMethod? CreateTransformMethod(
        Rule fromRule,
        IReadOnlyDictionary<NonOptionalType, TransformMethod> transforms,
        Grammar toGrammar)
    {
        var toRule = toGrammar.RuleFor(fromRule.Defines);
        // No way to make an auto transform, assume it is somehow covered
        if (toRule is null)
            return null;
        if (fromRule.IsTerminal)
            return new TransformTerminalMethod(this, fromRule.DefinesType, toRule.DefinesType);

        // Rules with children that cannot be transformed prevent the transform from being created
        foreach (var derivedRule in fromRule.DerivedRules)
        {
            // If there is a transform that covers the derived rule, that will be used. However, the
            // return type must be compatible.
            var existingTransformIncompatible = transforms.TryGetValue(derivedRule.DefinesType, out var transform)
                                                && !(transform.ToType?.IsSubtypeOf(toRule.DefinesType) ?? false);

            if (existingTransformIncompatible
                // If the derived rule has no matching rule in the new language, or the return type
                // is incompatible, then the transform cannot be created.
                || (!toGrammar.RuleFor(derivedRule.Defines)?.DefinesType.IsSubtypeOf(toRule.DefinesType) ?? true))
                return null;
        }

        return new TransformNonTerminalMethod(this, fromRule, fromRule.DefinesType, toRule.DefinesType);
    }

    private TransformCollectionMethod? CreateTransformMethod(CollectionType fromType, Grammar toGrammar)
    {
        var toRule = toGrammar.RuleFor(fromType.UnderlyingSymbol);
        // No way to make an auto transform, assume it is somehow covered
        if (toRule is null) return null;
        var toType = fromType.WithSymbol(toRule.Defines);
        return new TransformCollectionMethod(this, fromType, toType);
    }

    private IFixedList<Method> BubbleMethodParameters()
    {
        var methods = Methods.ToDictionary(m => m);
        var methodCallers = GetMethodCallers();

        var toBubble = new HashSet<Method>(methodCallers.Keys);
        while (toBubble.TryTake(out var calleeMethod))
        {
            var currentCalleeMethod = methods[calleeMethod];
            if (currentCalleeMethod.AdditionalParameters.Count == 0)
                // No parameters to bubble up
                continue;
            foreach (var callerMethod in methodCallers[calleeMethod])
            {
                if (callerMethod.ParametersDeclared)
                    continue; // Don't bubble to this method if parameters are declared with the pass

                var currentCallerMethod = methods[callerMethod];
                var bubbleChildProperties = callerMethod is not AdvancedCreateMethod
                    || calleeMethod is not SimpleCreateMethod;
                var adjustedAdditionalParameters = AdjustedAdditionalParameters(currentCalleeMethod, currentCallerMethod, bubbleChildProperties);
                var missingAndModifiedParameters = MissingAndModifiedParameters(currentCallerMethod.AdditionalParameters, adjustedAdditionalParameters);
                if (!missingAndModifiedParameters.Any())
                    // All parameters are already covered
                    continue;

                var additionalParameters = MergeByName(currentCallerMethod.AdditionalParameters, missingAndModifiedParameters).ToFixedList();
                methods[callerMethod] = currentCallerMethod with { AdditionalParameters = additionalParameters };
                // Caller modified, add to bubble if it is called by anything
                if (methodCallers.ContainsKey(callerMethod))
                    toBubble.Add(callerMethod);
            }
        }
        return methods.Values.ToFixedList();
    }

    private FixedDictionary<Method, IFixedSet<Method>> GetMethodCallers()
        => Methods.SelectMany(caller => caller.GetMethodsCalled().Select(callee => (caller, callee)))
                  .GroupToFixedDictionary(p => p.callee, pairs => pairs.Select(p => p.caller).ToFixedSet());

    private static IFixedList<Parameter> AdjustedAdditionalParameters(
        Method method,
        Method parentMethod,
        bool bubbleChildProperties)
    {
        var additionalParameters = method.AdditionalParameters;
        var fromType = method.FromType;
        var toType = parentMethod.FromType;
        if (!IsChildRule(fromType, toType))
            // Only in this case turn parameters into child parameters
            additionalParameters = additionalParameters.Select(p => p.ChildParameter).Distinct().ToFixedList();

        if (!bubbleChildProperties)
            additionalParameters = additionalParameters.Where(p => p.Type.UnderlyingSymbol is not InternalSymbol).ToFixedList();

        return additionalParameters;
    }

    private bool IsEntryTransform(TransformMethod transform)
    {
        var fromType = transform.From.Type;
        var toType = transform.To?.Type;
        if (toType is null) return true;
        return Equals(fromType, fromParameter?.Type) && Equals(toType, toParameter?.Type);
    }
}
