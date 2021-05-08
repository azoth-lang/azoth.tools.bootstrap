using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    /// <summary>
    /// A name of a variable or namespace
    /// </summary>
    internal class NameExpressionSyntax : ExpressionSyntax, INameExpressionSyntax
    {
        private LexicalScope? containingLexicalScope;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get =>
                containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }
        // A null name means this syntax was generated as an assumed missing name and the name is unknown
        public Name? Name { get; }
        public Promise<Symbol?> ReferencedSymbol { get; } = new Promise<Symbol?>();

        public NameExpressionSyntax(TextSpan span, Name? name)
            : base(span)
        {
            Name = name;
        }

        public IEnumerable<IPromise<Symbol>> LookupInContainingScope()
        {
            if (containingLexicalScope == null)
                throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

            // If name is unknown, no symbols
            if (Name is null) return Enumerable.Empty<IPromise<Symbol>>();

            return containingLexicalScope.Lookup(Name);
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
        public override string ToString()
        {
            return Name?.ToString() ?? "⧼unknown⧽";
        }
    }
}
