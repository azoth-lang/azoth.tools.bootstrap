using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal abstract class ExpressionSyntax : Syntax, IExpressionSyntax
    {
        /// <summary>
        /// If an expression has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { [DebuggerStepThrough] get; private set; }

        private DataType? dataType;
        [DisallowNull]
        public DataType? DataType
        {
            [DebuggerStepThrough]
            get => dataType;
            set
            {
                if (dataType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                dataType = value ?? throw new ArgumentNullException(nameof(DataType),
                           "Can't set type to null");
            }
        }

        public DataType? ConvertedDataType => ImplicitConversion is null ? dataType : ImplicitConversion.To;

        private Conversion? implicitConversion;
        [DisallowNull]
        public virtual Conversion? ImplicitConversion
        {
            [DebuggerStepThrough]
            get => implicitConversion;
            set
            {
                if (implicitConversion != null) throw new InvalidOperationException("Can't set conversion repeatedly");
                implicitConversion = value ?? throw new ArgumentNullException(nameof(ImplicitConversion), "Can't set conversion to null");
            }
        }

        private ExpressionSemantics? semantics;

        [DisallowNull]
        public ExpressionSemantics? Semantics
        {
            [DebuggerStepThrough]
            get => semantics;
            set
            {
                if (semantics != null)
                    throw new InvalidOperationException("Can't set semantics repeatedly");
                semantics = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        protected ExpressionSyntax(TextSpan span, ExpressionSemantics? semantics = null)
            : base(span)
        {
            this.semantics = semantics;
        }

        public void Poison()
        {
            Poisoned = true;
        }

        protected abstract OperatorPrecedence ExpressionPrecedence { get; }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
        }
    }
}
