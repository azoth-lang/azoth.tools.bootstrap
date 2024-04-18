using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

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
        EntryTransform = DeclaredTransforms.FirstOrDefault(IsEntryTransform) ?? CreateEntryTransform();
        Transforms = CreateTransforms();
    }

    private static Language? GetOrLoadLanguageNamed(SymbolNode? name, LanguageLoader languageLoader)
        => name is not null && !name.IsQuoted ? languageLoader.GetOrLoadLanguageNamed(name.Text) : null;

    private Parameter CreateFromParameter()
    {
        var fromType = Type.Create(FromLanguage?.Grammar,
            FromLanguage?.Entry ?? Symbol.Create(FromLanguage?.Grammar, Syntax.From));
        var fromParameter = Parameter.Create(fromType, "from");
        fromParameter ??= Parameter.Void;
        return fromParameter;
    }

    private Parameter CreateFromContextParameter()
    {
        var contextType = Type.Create(FromLanguage?.Grammar, FromContext);
        var contextParameter = Parameter.Create(contextType, "context");
        contextParameter ??= Parameter.Void;
        return contextParameter;
    }

    private Parameter CreateToParameter()
    {
        var toType = Type.Create(ToLanguage?.Grammar,
            ToLanguage?.Entry ?? Symbol.Create(ToLanguage?.Grammar, Syntax.To));
        var toParameter = Parameter.Create(toType, "to");
        toParameter ??= Parameter.Void;
        return toParameter;
    }
    private Parameter CreateToContextParameter()
    {
        var contextType = Type.Create(ToLanguage?.Grammar, ToContext);
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

        var coveredTransformPairs = transforms.Select(TransformTypePair.Create).WhereNotNull().ToFixedSet();

        var transformToPairs = ToLanguage?.Grammar.Rules.SelectMany(r => r.DeclaredProperties.Select(p => p.Type))
                                           .Where(t => t.Symbol.ReferencedRule?.IsModified ?? false)
                                           .Distinct(Type.EquivalenceComparer)
                                           .Select(CreateTransformTypePairFromTargetType)
                                           .Except(coveredTransformPairs)
                                           .ToFixedList() ?? FixedList.Empty<TransformTypePair>();

        //transforms.AddRange(transformToPairs.Select(p => p.ToTransform(this)));

        return transforms.ToFixedList();
    }

    private TransformTypePair CreateTransformTypePairFromTargetType(Type targetType)
    {
        var fromSymbol = targetType.Symbol.ReferencedRule!.ExtendsRule!.Defines;
        var fromType = Type.Create(FromLanguage!.Grammar, fromSymbol, targetType.CollectionKind);
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
