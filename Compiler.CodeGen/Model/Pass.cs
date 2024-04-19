using System;
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
        var fromType = FromLanguage is not null ? transform.From[0].Type : null;
        var toType = ToLanguage is not null ? transform.To[0].Type : null;
        if (fromType is null || toType is null)
            return true;
        return Type.AreEquivalent(fromType, fromParameter?.Type)
               && Type.AreEquivalent(toType, toParameter?.Type);
    }

    private Transform CreateEntryTransform()
        => new(this, Parameters(fromParameter), Parameters(toParameter), false);

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
                        var toSymbol = toGrammar?.RuleFor(parentFromType.UnderlyingSymbol)?.Defines;
                        var toType = toSymbol is not null ? parentFromType.WithSymbol(toSymbol) : null;
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

    private IEnumerable<NonVoidType> ParentTransformsFrom(NonVoidType fromType)
    {
        var grammar = FromLanguage!.Grammar;
        foreach (var rule in grammar.Rules)
        {
            if (rule.AllProperties.Any(p => p.Type == fromType))
                yield return new SymbolType(rule.Defines);

            if (fromType is not CollectionType)
                foreach (var property in rule.AllProperties.Where(p => p.Type.UnderlyingSymbol == fromType.UnderlyingSymbol
                                                                       && p.Type is CollectionType))
                    yield return property.Type;
        }
    }

    //private TransformTypePair CreateTransformTypePairFromTargetType(Type targetType)
    //{
    //    var fromSymbol = ((InternalSymbol)targetType.Symbol).ReferencedRule!.ExtendsRule!.Defines;
    //    var fromType = Type.Create(fromSymbol, targetType.CollectionKind);
    //    return new(fromType, targetType);
    //}

    private class TransformTypePair(NonVoidType from, NonVoidType to) : IEquatable<TransformTypePair>
    {
        public static TransformTypePair? Create(Transform transform)
        {
            var fromType = transform.From.FirstOrDefault()?.Type;
            var toType = transform.To.FirstOrDefault()?.Type;
            if (fromType is null || toType is null)
                return null;
            return new(fromType, toType);
        }

        public NonVoidType From { get; } = from;
        public NonVoidType To { get; } = to;

        public bool Equals(TransformTypePair? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            //return From.IsEquivalentTo(other.From) && To.IsEquivalentTo(other.To);
            throw new NotImplementedException();
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
