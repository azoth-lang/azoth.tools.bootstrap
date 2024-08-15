using System;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types.Type;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(Rule rule, string indent = "")
    {
        var children = rule.DerivedRules;
        if (!children.Any()) return "";
        var builder = new StringBuilder();
        builder.Append(indent);
        builder.AppendLine("[Closed(");
        bool first = true;
        foreach (var child in children)
        {
            if (first)
                first = false;
            else
                builder.AppendLine(",");
            builder.Append(indent);
            builder.Append($"    typeof({child.Defines.FullName})");
        }

        builder.AppendLine(")]");
        return builder.ToString();
    }

    /// <remarks><paramref name="root"/> is distinct from the default parent. The default parent has
    /// already been applied to the <see cref="Rule"/>. <paramref name="root"/> is used to apply
    /// a base interface outside of the default parent.</remarks>
    public static string BaseTypes(Rule rule, string? root = null)
    {
        var parents = rule.Parents.OfType<ExternalSymbol>().Select(p => p.FullName)
                          .Concat(rule.BaseRules.Select(r => TypeName(r.Defines)))
                          .ToFixedList();

        bool anyParents = parents.Any();
        if (!anyParents && root is null) return "";

        if (root is not null && !anyParents)
            parents = parents.Append(root).ToFixedList();

        return " : " + string.Join(", ", parents);
    }

    public static string TypeName(Symbol symbol)
        => symbol.FullName;

    public static string Type(Type type) => Type(type, TypeName);

    private static string Type(Type type, Func<Symbol, string> emitSymbol)
        => type switch
        {
            SymbolType t => emitSymbol(t.Symbol),
            ListType t => $"IFixedList<{Type(t.ElementType, emitSymbol)}>",
            SetType t => $"IFixedSet<{Type(t.ElementType, emitSymbol)}>",
            OptionalType t => $"{Type(t.UnderlyingType, emitSymbol)}?",
            VoidType _ => "void",
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static string PropertyIsNew(Property property)
        => property.IsNewDefinition ? "new " : "";
}
