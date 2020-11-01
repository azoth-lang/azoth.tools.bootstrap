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
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    /// <summary>
    /// The unqualified name of a type
    /// </summary>
    internal class TypeNameSyntax : TypeSyntax, ITypeNameSyntax
    {
        private LexicalScope? containingLexicalScope;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get => containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }
        public TypeName Name { get; }
        public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();

        public TypeNameSyntax(TextSpan span, TypeName name)
            : base(span)
        {
            Name = name;
        }

        public IEnumerable<IPromise<TypeSymbol>> LookupInContainingScope()
        {
            if (containingLexicalScope != null)
                return containingLexicalScope.Lookup(Name).Select(p => p.As<TypeSymbol>()).WhereNotNull();

            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
