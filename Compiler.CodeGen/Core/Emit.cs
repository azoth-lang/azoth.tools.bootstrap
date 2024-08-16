using System;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(TreeNodeModel node, string indent = "")
    {
        var children = node.ChildNodes;
        if (!children.Any()) return $"// [Closed(typeof({node.Defines.ClassName}))]{Environment.NewLine}";
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

    public static string BaseTypes(TreeNodeModel node)
    {
        var supertypes = node.Supertypes.OfType<ExternalSymbol>().Select(p => p.FullName)
                          .Concat(node.SupertypeNodes.Select(r => TypeName(r.Defines)))
                          .ToFixedList();

        if (supertypes.IsEmpty) return "";

        return " : " + string.Join(", ", supertypes);
    }

    public static string TypeName(Symbol symbol)
        => symbol.FullName;

    public static string Type(TypeModel type) => Type(type, TypeName);

    private static string Type(TypeModel type, Func<Symbol, string> emitSymbol)
        => type switch
        {
            SymbolType t => emitSymbol(t.Symbol),
            ListType t => $"IFixedList<{Type(t.ElementType, emitSymbol)}>",
            SetType t => $"IFixedSet<{Type(t.ElementType, emitSymbol)}>",
            OptionalType t => $"{Type(t.UnderlyingType, emitSymbol)}?",
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static string ClassName(InternalSymbol symbol) => symbol.ClassName;

    public static string PropertyIsNew(PropertyModel property)
        => property.IsNewDefinition ? "new " : "";

    public static string ParameterName(PropertyModel property)
        => property.Name.ToCamelCase();

    public static string Body(SynthesizedAttributeModel attribute)
    {
        if (attribute.DefaultExpression is not null)
            return $"{Environment.NewLine}        => {attribute.DefaultExpression};";
        if (!string.IsNullOrEmpty(attribute.Parameters))
            return ";";

        return " { get; }";
    }
}
