using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypeSyntax : CodeSyntax, ITypeSyntax
{
    private DataType? namedType;

    [DisallowNull]
    public DataType? NamedType
    {
        get => namedType;
        set
        {
            if (namedType is not null)
                throw new InvalidOperationException("Can't set type repeatedly");
            namedType = value ?? throw new ArgumentNullException(nameof(NamedType),
                "Can't set type to null");
        }
    }

    protected TypeSyntax(TextSpan span)
        : base(span)
    {
    }

    // Useful for debugging
    public abstract override string ToString();
}
