using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
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

    public Parameter FromParameter { get; }
    public Parameter FromContextParameter { get; }
    public Parameter ToParameter { get; }
    public Parameter ToContextParameter { get; }
    public IFixedList<Parameter> FullRunParameters { get; }
    public IFixedList<Parameter> RunParameters { get; }
    public IFixedList<Parameter> FullRunReturn { get; }
    public IFixedList<Parameter> RunReturn { get; }

    public IFixedList<Transform> DeclaredTransforms { get; }
    public Transform EntryTransform { get; }
    public IFixedList<Transform> Transforms { get; }

    public Pass(PassNode syntax, LanguageLoader languageLoader)
    {
        Syntax = syntax;
        FromLanguage = GetOrLoadLanguageNamed(Syntax.From, languageLoader);
        ToLanguage = GetOrLoadLanguageNamed(Syntax.To, languageLoader);
        FromContext = Symbol.Create(FromLanguage?.Grammar, Syntax.FromContext);
        ToContext = Symbol.Create(ToLanguage?.Grammar, Syntax.ToContext);
        FromParameter = CreateFromParameter();
        FromContextParameter = CreateFromContextParameter();
        FullRunParameters = Parameters(FromParameter, FromContextParameter);
        RunParameters = RemoveVoid(FullRunParameters);
        ToParameter = CreateToParameter();
        ToContextParameter = CreateToContextParameter();
        FullRunReturn = Parameters(ToParameter, ToContextParameter);
        RunReturn = RemoveVoid(FullRunReturn);
        DeclaredTransforms = Syntax.Transforms.Select(t => new Transform(this, t)).ToFixedList();
        EntryTransform = DeclaredTransforms.SingleOrDefault(IsEntryTransform) ?? CreateEntryTransform();
        Transforms = CreateTransforms();
    }

    private static Language? GetOrLoadLanguageNamed(SymbolNode? name, LanguageLoader languageLoader)
        => name is not null && !name.IsQuoted ? languageLoader.GetOrLoadLanguageNamed(name.Text) : null;

    private Parameter CreateFromParameter()
    {
        var fromType = Type.Create(FromLanguage?.Entry ?? Symbol.Create(FromLanguage?.Grammar, Syntax.From));
        var fromParameter = Parameter.Create(fromType, "from");
        fromParameter ??= Parameter.Void;
        return fromParameter;
    }

    private Parameter CreateFromContextParameter()
    {
        var contextType = Type.Create(FromContext);
        var contextParameter = Parameter.Create(contextType, "context");
        contextParameter ??= Parameter.Void;
        return contextParameter;
    }

    private Parameter CreateToParameter()
    {
        var toType = Type.Create(ToLanguage?.Entry ?? Symbol.Create(ToLanguage?.Grammar, Syntax.To));
        var toParameter = Parameter.Create(toType, "to");
        toParameter ??= Parameter.Void;
        return toParameter;
    }
    private Parameter CreateToContextParameter()
    {
        var contextType = Type.Create(ToContext);
        var contextParameter = Parameter.Create(contextType, "toContext");
        contextParameter ??= Parameter.Void;
        return contextParameter;
    }

    private static IFixedList<Parameter> Parameters(params Parameter?[] parameters)
        => parameters.WhereNotNull().ToFixedList();

    private static IFixedList<Parameter> NonVoidParameters(params Parameter[] parameters)
        => RemoveVoid(parameters);

    private static IFixedList<Parameter> RemoveVoid(IEnumerable<Parameter> parameters)
        => parameters.Where(p => p != Parameter.Void).ToFixedList();

    private bool IsEntryTransform(Transform transform)
    {
        var fromType = FromLanguage is not null ? transform.From[0].Type : null;
        var toType = ToLanguage is not null ? transform.To[0].Type : null;
        return (fromType?.IsEquivalentTo(FromParameter.Type) ?? true)
            && (toType?.IsEquivalentTo(ToParameter.Type) ?? true);
    }

    private Transform CreateEntryTransform()
        => new(this, NonVoidParameters(FromParameter), NonVoidParameters(ToParameter), false);

    private IFixedList<Transform> CreateTransforms()
    {
        var transforms = new List<Transform>(DeclaredTransforms.Prepend(EntryTransform).Distinct());

        if (FromLanguage is not null)
        {
            var coveredFromTypes = transforms.Select(t => t.From[0].Type).ToFixedSet();
            var newTransforms = new Dictionary<Type, Transform>();
            // bubble additional parameters upwards
            foreach (var declaredTransform in DeclaredTransforms)
                BubbleParametersUp(declaredTransform);

            transforms.AddRange(newTransforms.Values);

            void BubbleParametersUp(Transform transform)
            {
                var fromType = transform.From[0].Type;
                var additionalParameters = transform.From.Skip(1).ToFixedList();
                if (additionalParameters.Count == 0) return;
                var parentTransformsFrom = ParentTransformsFrom(fromType).Except(coveredFromTypes);
                var toGrammar = ToLanguage?.Grammar;
                foreach (var parentFromType in parentTransformsFrom)
                {
                    if (newTransforms.TryGetValue(parentFromType, out var parentTransform))
                    {
                        var missingParameters = additionalParameters.Except(parentTransform.From).ToFixedList();
                        if (!missingParameters.Any())
                        {
                            // All parameters are already covered
                            continue;
                        }

                        // Need to create a new transform with the additional parameters
                        parentTransform = new Transform(this,
                            parentTransform.From.Concat(missingParameters),
                            parentTransform.To, true);
                    }
                    else
                    {
                        var fromParameter = Parameter.Create(parentFromType, "from");
                        var toSymbol = toGrammar?.RuleFor(parentFromType.Symbol.ShortName)?.Defines;
                        var toType = Type.Create(toSymbol, parentFromType.CollectionKind);
                        var toParameter = Parameter.Create(toType, "to");
                        if (toParameter is null)
                            // No way to make an auto transform, assume it is somehow covered
                            continue;
                        parentTransform = new Transform(this,
                            additionalParameters.Prepend(fromParameter),
                            Parameters(toParameter), true);
                    }

                    // Put new transform in the dictionary
                    newTransforms[parentFromType] = parentTransform;

                    // Now bubble up the additional parameters
                    BubbleParametersUp(parentTransform);
                }
            }
        }

        //var coveredTransformPairs = transforms.Select(TransformTypePair.Create).WhereNotNull().ToFixedSet();

        //var transformToPairs = ToLanguage?.Grammar.Rules.SelectMany(r => r.DeclaredProperties.Select(p => p.Type))
        //                                   .Where(t => t.Symbol.ReferencedRule?.IsModified ?? false)
        //                                   .Distinct()
        //                                   .Select(CreateTransformTypePairFromTargetType)
        //                                   .Except(coveredTransformPairs)
        //                                   .ToFixedList() ?? FixedList.Empty<TransformTypePair>();

        //transforms.AddRange(transformToPairs.Select(p => p.ToTransform(this)));

        return transforms.ToFixedList();
    }

    private IEnumerable<Type> ParentTransformsFrom(Type fromType)
    {
        var grammar = FromLanguage!.Grammar;
        foreach (var rule in grammar.Rules)
        {
            if (rule.AllProperties.Any(p => p.Type == fromType))
                yield return Type.Create(rule.Defines);

            if (fromType.CollectionKind == CollectionKind.None)
                foreach (var property in rule.AllProperties.Where(p => p.Type.Symbol == fromType.Symbol && p.Type.IsCollection))
                    yield return property.Type;
        }
    }

    private TransformTypePair CreateTransformTypePairFromTargetType(Type targetType)
    {
        var fromSymbol = targetType.Symbol.ReferencedRule!.ExtendsRule!.Defines;
        var fromType = Type.Create(fromSymbol, targetType.CollectionKind);
        return new(fromType, targetType);
    }

    private class TransformTypePair(Type from, Type to) : IEquatable<TransformTypePair>
    {
        public static TransformTypePair? Create(Transform transform)
        {
            var fromType = transform.From.FirstOrDefault()?.Type;
            var toType = transform.To.FirstOrDefault()?.Type;
            if (fromType is null || toType is null)
                return null;
            return new(fromType, toType);
        }

        public Type From { get; } = from;
        public Type To { get; } = to;

        public bool Equals(TransformTypePair? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return From.IsEquivalentTo(other.From) && To.IsEquivalentTo(other.To);
        }

        public override bool Equals(object? obj)
            => obj is TransformTypePair other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(From, To);

        public static bool operator ==(TransformTypePair? left, TransformTypePair? right) => Equals(left, right);

        public static bool operator !=(TransformTypePair? left, TransformTypePair? right) => !Equals(left, right);

        public Transform ToTransform(Pass pass)
            => new Transform(pass,
                Parameter.Create(From, "from").Yield(),
                Parameter.Create(To, "to").Yield(),
                autoGenerate: true);
    }
}
