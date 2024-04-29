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
    public IFixedList<Transform> DeclaredTransforms { get; }
    public IFixedList<Parameter> RunParameters { get; }
    public IFixedList<Parameter> RunReturnValues { get; }
    public IFixedList<Parameter> PassParameters { get; }
    public Transform EntryTransform { get; }
    public IFixedList<Transform> Transforms { get; }

    public IFixedList<SimpleCreateMethod> SimpleCreateMethods { get; }
    public IFixedList<AdvancedCreateMethod> AdvancedCreateMethods { get; }
    public IFixedList<TransformMethod> TransformMethods { get; }
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
        DeclaredTransforms = Syntax.Transforms.Select(t => new Transform(this, t)).ToFixedList();
        EntryTransform = DeclaredTransforms.FirstOrDefault(IsEntryTransform) ?? CreateEntryTransform();
        Transforms = CreateTransforms();
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
    }

    private static Language? GetOrLoadLanguageNamed(SymbolNode? name, LanguageLoader languageLoader)
        => name is not null && !name.IsQuoted ? languageLoader.GetOrLoadLanguageNamed(name.Text) : null;

    private Parameter? CreateFromParameter()
    {
        var fromType = SymbolType.Create(FromLanguage?.Entry ?? Symbol.CreateExternalFromSyntax(Syntax.From));
        var fromParameter = Parameter.Create(fromType, "from");
        return fromParameter;
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
        var toParameter = Parameter.Create(toType, "to");
        return toParameter;
    }

    private Parameter? CreateToContextParameter()
    {
        var contextType = SymbolType.Create(ToContext);
        var contextParameter = Parameter.Create(contextType, "toContext");
        return contextParameter;
    }

    private static IFixedList<Parameter> Parameters(params Parameter?[] parameters)
        => parameters.WhereNotNull().ToFixedList();

    private bool IsEntryTransform(Transform transform)
    {
        var fromType = transform.From?.Type;
        var toType = transform.To?.Type;
        if (fromType is null || toType is null)
            return true;
        return Equals(fromType, fromParameter?.Type)
               && Equals(toType, toParameter?.Type);
    }

    private Transform CreateEntryTransform()
        => new(this, fromParameter, FixedList.Empty<Parameter>(), toParameter, FixedList.Empty<Parameter>(), false);

    private IFixedList<Transform> CreateTransforms()
    {
        var transforms = new List<Transform>(DeclaredTransforms);

        if (FromLanguage is not null)
        {
            var declaredFromTypes = DeclaredTransforms
                                    .Select(t => t.From?.Type.ToNonOptional()).WhereNotNull()
                                    .ToFixedSet();
            var newTransforms = new Dictionary<NonOptionalType, Transform>();
            if (!EntryTransform.IsDeclared)
                newTransforms.Add(EntryTransform.From!.Type.ToNonOptional(), EntryTransform);

            AddTransformsForModifiedTerminals(declaredFromTypes, newTransforms);

            // Bubble additional parameters upwards
            foreach (var transform in transforms.Concat(newTransforms.Values).ToFixedList())
                BubbleParametersUp(transform, declaredFromTypes, newTransforms);

            AddChildTransformsAsNeeded(DeclaredTransforms, newTransforms);

            // Bubble additional parameters upwards now that additional children have been added
            foreach (var transform in transforms.Concat(newTransforms.Values).ToFixedList())
                BubbleParametersUp(transform, declaredFromTypes, newTransforms);

            transforms.AddRange(newTransforms.Values);
        }

        return transforms.ToFixedList();
    }

    private void BubbleParametersUp(
        Transform transform,
        IFixedSet<NonOptionalType> declaredFromTypes,
        Dictionary<NonOptionalType, Transform> newTransforms)
    {
        var fromType = transform.From!.Type.ToNonOptional();
        if (transform.AdditionalParameters.Count == 0) return;

        var allFromTypes = declaredFromTypes.Concat(newTransforms.Keys).ToFixedSet();
        var parentTransformsFrom = ParentTransformsFrom(fromType, allFromTypes)
                                   .Except(declaredFromTypes).Except(fromType)
                                   .Distinct()
                                   .ToFixedList();

        var toGrammar = ToLanguage?.Grammar;
        foreach (var parentFromType in parentTransformsFrom)
        {
            var parentLookupType = parentFromType.ToNonOptional();

            // If there isn't a parent transform, create a simple one so we can then bubble up into it
            if (!newTransforms.TryGetValue(parentLookupType, out var parentTransform))
            {
                var toRule = toGrammar?.RuleFor(parentFromType.UnderlyingSymbol);
                if (toRule is null)
                    // No way to make an auto transform, assume it is somehow covered
                    continue;
                var isSimpleTransform = parentLookupType is SymbolType && toRule.IsTerminal;
                var additionalParameters = isSimpleTransform
                    ? toRule.ModifiedProperties.Select(p => p.Parameter)
                    : Enumerable.Empty<Parameter>();
                parentTransform = CreateTransform(parentFromType, additionalParameters, toRule, forceAutoGenerate: true);
                newTransforms.Add(parentLookupType, parentTransform);
            }

            // TODO this is really about optional, clarify to make code readable
            bool fromTypeChanged = !parentFromType.IsSubtypeOf(parentTransform.From!.Type);
            var parentAdditionalParameters = parentTransform.AdditionalParameters;
            var adjustedAdditionalParameters = AdjustedAdditionalParameters(transform, parentTransform);
            var missingAndModifiedParameters
                = MissingAndModifiedParameters(parentAdditionalParameters, adjustedAdditionalParameters);

            // We have to re-create the transform if there are missing parameters OR
            // if the from type has changed (e.g. from non-optional to optional).
            if (!missingAndModifiedParameters.Any() && !fromTypeChanged)
            {
                // All parameters are already covered
                continue;
            }

            var parentFrom = parentTransform.From;
            var parentReturnValues = parentTransform.AllReturnValues;
            var parentTo = parentTransform.To;
            if (fromTypeChanged)
            {
                parentFrom = Parameter.Create(parentFromType, "from");
                // Make the to type optional if the parent from type is optional
                var toType = new OptionalType(parentReturnValues[0].Type);
                parentTo = Parameter.Create(toType, "to");
            }

            // Need to create a new transform with the additional parameters
            parentTransform = new Transform(this, parentFrom,
                MergeByName(parentAdditionalParameters, missingAndModifiedParameters), parentTo,
                parentTransform.AdditionalReturnValues, parentTransform.AutoGenerate);

            // Put new transform in the dictionary
            newTransforms[parentLookupType] = parentTransform;

            // Now bubble up the additional parameters
            BubbleParametersUp(parentTransform, declaredFromTypes, newTransforms);
        }
    }

    private static IEnumerable<Parameter> MergeByName(
        IEnumerable<Parameter> parentAdditionalParameters,
        IEnumerable<Parameter> missingAndModifiedParameters)
        => parentAdditionalParameters.Concat(missingAndModifiedParameters).MergeByName();

    private static IFixedList<Parameter> AdjustedAdditionalParameters(Transform transform, Transform parentTransform)
    {
        var additionalParameters = transform.AdditionalParameters;
        var fromType = transform.From?.Type;
        var toType = parentTransform.From?.Type;
        if (!IsChildRule(fromType, toType))
            // Only in this case turn parameters into child parameters
            additionalParameters = additionalParameters.Select(p => p.ChildParameter).Distinct().ToFixedList();

        return additionalParameters;
    }

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

    private void AddTransformsForModifiedTerminals(
        IFixedSet<NonOptionalType> declaredFromTypes,
        Dictionary<NonOptionalType, Transform> newTransforms)
    {
        if (ToLanguage is null || FromLanguage == ToLanguage)
            return;

        // Add auto transforms for all rules that are modified terminals
        var modifiedRules = ToLanguage.Grammar.Rules.Where(r => r is { IsModified: true, IsTerminal: true });
        foreach (var toRule in modifiedRules)
        {
            var fromRule = toRule.ExtendsRule!;
            var fromType = new SymbolType(fromRule.Defines);
            if (declaredFromTypes.Contains(fromType)
                || declaredFromTypes.Any(fromType.IsSubtypeOf))
                continue;

            var additionalParameters = toRule.ModifiedProperties.Select(p => p.Parameter);
            newTransforms[fromType] = CreateTransform(fromType, additionalParameters, toRule, forceAutoGenerate: true);
        }
    }

    private void AddChildTransformsAsNeeded(
        IFixedList<Transform> declaredTransforms,
        Dictionary<NonOptionalType, Transform> newTransforms)
    {
        var fromTypesCoveredByDeclaredTransforms = declaredTransforms
                                                   .Select(t => t.From?.Type.ToNonOptional())
                                                   .WhereNotNull().ToFixedSet();
        var unprocessedTransforms = new Queue<Transform>(declaredTransforms.Concat(newTransforms.Values));

        while (unprocessedTransforms.TryDequeue(out var transform))
        {
            var fromType = transform.From?.Type;
            if (fromType?.UnderlyingSymbol is not InternalSymbol { ReferencedRule: { IsTerminal: true } rule })
                continue;

            foreach (var childRule in rule.DerivedRules)
            {
                var childFromType = new SymbolType(childRule.Defines);
                if (fromTypesCoveredByDeclaredTransforms.Contains(childFromType)
                    || newTransforms.ContainsKey(childFromType))
                    continue;
                // Child type isn't covered yet, add a transform for it
                var childTransform = CreateTransform(childFromType, ToLanguage?.Grammar, forceAutoGenerate: true);
                if (childTransform is null) continue;

                newTransforms.Add(childFromType, childTransform);
                if (!childRule.IsTerminal)
                    unprocessedTransforms.Enqueue(childTransform);
            }
        }
    }

    private Transform? CreateTransform(
        NonVoidType fromType,
        Grammar? toGrammar,
        IEnumerable<Parameter>? additionalParameters = null,
        bool forceAutoGenerate = false)
    {
        var toRule = toGrammar?.RuleFor(fromType.UnderlyingSymbol);
        if (toRule is null)
            // No way to make an auto transform, assume it is somehow covered
            return null;
        additionalParameters ??= Enumerable.Empty<Parameter>();
        var isSimpleTransform = fromType.ToNonOptional() is SymbolType;
        var modifiedParameters = isSimpleTransform
            ? toRule.ModifiedProperties.Select(p => p.Parameter)
            : Enumerable.Empty<Parameter>();
        return CreateTransform(fromType, MergeByName(modifiedParameters, additionalParameters),
            toRule, forceAutoGenerate);
    }

    private Transform CreateTransform(
        NonVoidType fromType,
        IEnumerable<Parameter>? additionalParameters,
        Rule toRule,
        bool forceAutoGenerate = false)
    {
        var fromParameter = Parameter.Create(fromType, "from");
        additionalParameters ??= Enumerable.Empty<Parameter>();
        var toSymbol = toRule.Defines;
        var toType = fromType.WithSymbol(toSymbol);
        var toParameter = Parameter.Create(toType, "to");
        bool autoGenerate = forceAutoGenerate || fromType is CollectionType || !toRule.IsModified || !toRule.IsTerminal;
        var transform = new Transform(this, fromParameter, additionalParameters, toParameter,
            FixedList.Empty<Parameter>(), autoGenerate);
        return transform;
    }

    private IEnumerable<NonVoidType> ParentTransformsFrom(
        NonOptionalType fromType,
        IFixedSet<NonOptionalType> allFromTypes)
    {
        var grammar = FromLanguage!.Grammar;
        if (fromType is SymbolType { Symbol: InternalSymbol symbol })
            foreach (var rule in symbol.ReferencedRule.BaseRules)
            {
                var parentFromType = new SymbolType(rule.Defines);
                // Only bubble up if the parent type already exists to avoid adding transforms that
                // don't make sense because the declared transforms don't fit.
                if (allFromTypes.Contains(parentFromType))
                    yield return parentFromType;
            }

        foreach (var rule in grammar.Rules)
        {
            if (rule.AllProperties.Any(p => fromType.IsSubtypeOf(p.Type)))
                yield return new SymbolType(rule.Defines);

            foreach (var type in rule.AllProperties.Select(p => p.Type).Where(fromType.IsSubtypeOf))
                yield return type;

            if (fromType is SymbolType { Symbol: InternalSymbol fromSymbol })
                foreach (var type in rule.AllProperties
                                         .Select(p => p.Type)
                                         .OfType<CollectionType>()
                                         .Where(t => fromType.IsSubtypeOf(t.ElementType)))
                {
                    // Only take one step up at a time so that additional parameters are bubbled up correctly
                    if (type.ElementType == fromType)
                        yield return type;
                    else
                    {
                        var parentSymbol = ParentSymbol(fromSymbol, type.ElementType.UnderlyingSymbol);
                        if (parentSymbol is not null)
                            yield return new SymbolType(parentSymbol);
                    }
                }
        }
    }

    private static Symbol? ParentSymbol(InternalSymbol from, Symbol toward)
    {
        var parentSymbols = from.ReferencedRule.BaseRules.Select(r => r.Defines).ToFixedSet();
        if (parentSymbols.Contains(toward))
            return toward;
        return parentSymbols.FirstOrDefault(sym => ParentSymbol(sym, toward) is not null);
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

        foreach (var rule in ToLanguage.Grammar.Rules.Where(ShouldCreate))
            if (rule.IsTerminal)
                yield return new AdvancedCreateTerminalMethod(this, rule);
            else
                yield return new AdvancedCreateNonTerminalMethod(this, rule);

        yield break;

        static bool ShouldCreate(Rule r)
            => r is { ExtendsRule: not null }
               && (!r.IsTerminal || r.DifferentChildProperties.Any());
    }

    private IEnumerable<TransformMethod> CreateTransformMethods()
    {
        var transforms = CreateDeclaredTransformMethods().ToDictionary(t => t.FromCoreType);

        if (!IsLanguageTransform || FromLanguage is null)
            return transforms.Values;

        var fromGrammar = FromLanguage.Grammar;
        var toGrammar = ToLanguage.Grammar;

        // Collect the values to avoid modifying the dictionary while iterating
        foreach (var baseTransform in CreateTransformsModifiedTerminals(transforms, toGrammar).ToArray())
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
        foreach (var transform in DeclaredTransforms)
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

    private IEnumerable<TransformMethod> CreateTransformsModifiedTerminals(
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

            var transform = CreateTransformMethod(fromRule, toGrammar);
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

            var parentTransform = CreateTransformMethod(baseRule, toGrammar);

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
        foreach (var rule in fromGrammar.Rules.Where(r => !transforms.ContainsKey(r.DefinesType)))
        {
            if (rule.AllProperties.Any(p => p.Type == fromCoreType))
            {
                var parentTransform = CreateTransformMethod(rule, toGrammar);

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
            var derivedTransform = CreateTransformMethod(derivedRule, toGrammar);
            if (derivedTransform is null)
                continue;

            yield return derivedTransform;
        }
    }

    private TransformMethod? CreateTransformMethod(Rule fromRule, Grammar toGrammar)
    {
        var toRule = toGrammar.RuleFor(fromRule.Defines);
        // No way to make an auto transform, assume it is somehow covered
        if (toRule is null)
            return null;
        if (fromRule.IsTerminal)
            return new TransformTerminalMethod(this, fromRule.DefinesType, toRule.DefinesType);

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
    {
        return Methods.SelectMany(caller => caller.GetMethodsCalled().Select(callee => (caller, callee)))
                      .GroupToFixedDictionary(p => p.callee, pairs => pairs.Select(p => p.caller).ToFixedSet());
    }

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
}
