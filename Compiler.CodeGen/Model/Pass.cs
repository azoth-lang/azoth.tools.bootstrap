using System.Collections.Generic;
using System.Linq;
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

    public Pass(PassNode syntax, LanguageLoader languageLoader)
    {
        Syntax = syntax;
        FromLanguage = GetOrLoadLanguageNamed(Syntax.From, languageLoader);
        ToLanguage = GetOrLoadLanguageNamed(Syntax.To, languageLoader);
        FromContext = Symbol.CreateExternal(Syntax.FromContext);
        ToContext = Symbol.CreateExternal(Syntax.ToContext);
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
        var transforms = new List<Transform>(DeclaredTransforms.Prepend(EntryTransform).Distinct());

        if (FromLanguage is not null)
        {
            var declaredFromTypes = transforms.Select(t => t.From?.Type.ToNonOptional()).WhereNotNull().ToFixedSet();
            var newTransforms = new Dictionary<NonOptionalType, Transform>();

            AddTransformsForModifiedTerminals(declaredFromTypes, newTransforms);

            // Bubble additional parameters upwards
            foreach (var transform in transforms.Concat(newTransforms.Values).ToFixedList())
                BubbleParametersUp(transform, declaredFromTypes, newTransforms);

            AddChildTransformsAsNeeded(transforms, declaredFromTypes, newTransforms);

            // Bubble additional parameters upwards now that additional children have been added
            foreach (var transform in transforms.Concat(newTransforms.Values).ToFixedList())
                BubbleParametersUp(transform, declaredFromTypes, newTransforms);

            transforms.AddRange(newTransforms.Values);
        }

        return transforms.ToFixedList();
    }

    private void BubbleParametersUp(Transform transform, IFixedSet<NonOptionalType> declaredFromTypes,
        Dictionary<NonOptionalType, Transform> newTransforms)
    {
        var fromType = transform.From!.Type.ToNonOptional();
        var additionalParameters = transform.AdditionalParameters;
        if (additionalParameters.Count == 0) return;

        var allFromTypes = declaredFromTypes.Concat(newTransforms.Keys).ToFixedSet();
        var parentTransformsFrom = ParentTransformsFrom(fromType, allFromTypes)
                                   .Except(declaredFromTypes).Except(fromType)
                                   .Where(t => !declaredFromTypes.Any(t.IsSubtypeOf))
                                   .Distinct()
                                   .ToFixedList();

        var toGrammar = ToLanguage?.Grammar;
        foreach (var parentFromType in parentTransformsFrom)
        {
            var parentLookupType = parentFromType.ToNonOptional();

            if (newTransforms.TryGetValue(parentLookupType, out var parentTransform))
            {
                bool fromTypeChanged = !parentFromType.IsSubtypeOf(parentTransform.From!.Type);
                var parentAdditionalParameters = parentTransform.AdditionalParameters;
                var missingParameters = additionalParameters.Except(parentAdditionalParameters)
                                                            .Where(p => parentAdditionalParameters.All(pp => pp.Name != p.Name))
                                                            .ToFixedList();

                // We have to re-create the transform if there are missing parameters OR
                // if the from type has changed (e.g. from non-optional to optional).
                if (!missingParameters.Any() && !fromTypeChanged)
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
                parentTransform = new Transform(this,
                    parentFrom,
                    parentTransform.AdditionalParameters.Concat(missingParameters),
                    parentTo, parentTransform.AdditionalReturnValues,
                    parentTransform.AutoGenerate);
            }
            else
            {
                parentTransform = CreateTransform(parentFromType, toGrammar, additionalParameters);
                if (parentTransform is null)
                    // Couldn't create transform, assume it is somehow covered
                    continue;
            }

            // Put new transform in the dictionary
            newTransforms[parentLookupType] = parentTransform;

            // Now bubble up the additional parameters
            BubbleParametersUp(parentTransform, declaredFromTypes, newTransforms);
        }
    }

    private void AddTransformsForModifiedTerminals(IFixedSet<NonOptionalType> declaredFromTypes,
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

            var additionalParameters = toRule.ModifiedProperties.Select(p => p.ToParameter());
            newTransforms[fromType] = CreateTransform(fromType, additionalParameters, toRule, forceAutoGenerate: true);
        }
    }

    private void AddChildTransformsAsNeeded(
        IReadOnlyList<Transform> transforms,
        IFixedSet<NonOptionalType> declaredFromTypes,
        Dictionary<NonOptionalType, Transform> newTransforms)
    {
        var allTransforms = transforms.Concat(newTransforms.Values).ToFixedSet();
        var coveredFromTypes = allTransforms.Select(t => t.From?.Type).WhereNotNull().ToHashSet();
        var unprocessedTransforms = new Queue<Transform>(allTransforms);

        while (unprocessedTransforms.TryDequeue(out var transform))
        {
            var fromType = transform.From?.Type;
            if (fromType?.UnderlyingSymbol is not InternalSymbol fromSymbol) continue;

            var rule = fromSymbol.ReferencedRule;
            if (rule.IsTerminal) continue;

            foreach (var childRule in rule.ChildRules)
            {
                var childFromType = new SymbolType(childRule.Defines);
                if (coveredFromTypes.Contains(childFromType)
                    || coveredFromTypes.Any(t => t is OptionalType optionalType && optionalType.UnderlyingType == childFromType)
                    || declaredFromTypes.Any(childFromType.IsSubtypeOf))
                    continue;
                // Child type isn't covered yet, add a transform for it
                var childTransform = CreateTransform(childFromType, ToLanguage?.Grammar);
                if (childTransform is null) continue;

                newTransforms.Add(childFromType, childTransform);
                if (!childRule.IsTerminal)
                {
                    unprocessedTransforms.Enqueue(childTransform);
                    coveredFromTypes.Add(childFromType);
                }
            }
        }
    }

    private Transform? CreateTransform(
        NonVoidType fromType,
        Grammar? toGrammar,
        IEnumerable<Parameter>? additionalParameters = null)
    {
        var toRule = toGrammar?.RuleFor(fromType.UnderlyingSymbol);
        if (toRule is null)
            // No way to make an auto transform, assume it is somehow covered
            return null;
        return CreateTransform(fromType, additionalParameters, toRule);
    }

    private Transform CreateTransform(NonVoidType fromType, IEnumerable<Parameter>? additionalParameters, Rule toRule, bool forceAutoGenerate = false)
    {
        var fromParameter = Parameter.Create(fromType, "from");
        additionalParameters ??= Enumerable.Empty<Parameter>();
        var toSymbol = toRule.Defines;
        var toType = fromType.WithSymbol(toSymbol);
        var toParameter = Parameter.Create(toType, "to");
        bool autoGenerate = forceAutoGenerate || fromType is CollectionType || !toRule.IsModified || !toRule.IsTerminal;
        var transform = new Transform(this, fromParameter, additionalParameters, toParameter, FixedList.Empty<Parameter>(),
            autoGenerate);
        return transform;
    }

    private IEnumerable<NonVoidType> ParentTransformsFrom(NonOptionalType fromType, IFixedSet<NonOptionalType> allFromTypes)
    {
        var grammar = FromLanguage!.Grammar;
        if (fromType is SymbolType { Symbol: InternalSymbol symbol })
            foreach (var rule in symbol.ReferencedRule.ParentRules)
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

            if (fromType is not CollectionType)
                foreach (var type in rule.AllProperties
                                             .Select(p => p.Type)
                                             .OfType<CollectionType>()
                                             .Where(t => fromType.IsSubtypeOf(t.ElementType)))
                {
                    yield return type;
                    yield return type.ElementType;
                }
        }
    }
}
