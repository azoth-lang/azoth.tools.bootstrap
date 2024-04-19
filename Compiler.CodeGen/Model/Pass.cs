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
                var parentTransformsFrom = ParentTransformsFrom(fromType)
                                           .Except(coveredFromTypes)
                                           .Where(t => !coveredFromTypes.Any(t.IsSubtypeOf));
                var toGrammar = ToLanguage?.Grammar;
                foreach (var parentFromType in parentTransformsFrom)
                {
                    if (newTransforms.TryGetValue(parentFromType, out var parentTransform))
                    {
                        var parentParameters = parentTransform.From;
                        var missingParameters = additionalParameters.Except(parentParameters)
                                                                    .Where(p => parentParameters.All(pp => pp.Name != p.Name))
                                                                    .ToFixedList();
                        if (!missingParameters.Any())
                        {
                            // All parameters are already covered
                            continue;
                        }

                        // Need to create a new transform with the additional parameters
                        parentTransform = new Transform(this,
                            parentParameters.Concat(missingParameters),
                            parentTransform.To, true);
                    }
                    else
                    {
                        var fromParameter = Parameter.Create(parentFromType, "from");
                        var toRule = toGrammar?.RuleFor(parentFromType.UnderlyingSymbol);
                        var toSymbol = toRule?.Defines;
                        var toType = toSymbol is not null ? parentFromType.WithSymbol(toSymbol) : null;
                        var toParameter = Parameter.Create(toType, "to");
                        if (toParameter is null)
                            // No way to make an auto transform, assume it is somehow covered
                            continue;
                        parentTransform = new Transform(this,
                            additionalParameters.Prepend(fromParameter),
                            Parameters(toParameter), parentFromType is CollectionType || !toRule!.IsModified);
                    }

                    // Put new transform in the dictionary
                    newTransforms[parentFromType] = parentTransform;

                    // Now bubble up the additional parameters
                    BubbleParametersUp(parentTransform);
                }
            }
        }

        return transforms.ToFixedList();
    }

    private IEnumerable<NonVoidType> ParentTransformsFrom(NonVoidType fromType)
    {
        var grammar = FromLanguage!.Grammar;
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
                }
        }
    }
}
