using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST
{
    public static class ExpressionSemanticsExtensions
    {
        public static string Action(this ExpressionSemantics valueSemantics)
        {
            string mutability = valueSemantics switch
            {
                ExpressionSemantics.Never => "never",
                ExpressionSemantics.Void => "void",
                ExpressionSemantics.MoveValue => "move",
                ExpressionSemantics.CopyValue => "copy",
                ExpressionSemantics.IsolatedReference => "iso",
                ExpressionSemantics.MutableReference => "mut",
                ExpressionSemantics.ReadOnlyReference => "read",
                ExpressionSemantics.ConstReference => "const",
                ExpressionSemantics.IdReference => "id",
                ExpressionSemantics.CreateReference => "ref",
                _ => throw ExhaustiveMatch.Failed(valueSemantics),
            };

            return mutability;
        }

        /// <summary>
        /// Validates that expression semantics have been assigned.
        /// </summary>
        [DebuggerHidden]
        public static ExpressionSemantics Assigned(this ExpressionSemantics? semantics)
        {
            return semantics ?? throw new InvalidOperationException("Expression semantics not assigned");
        }
    }
}
