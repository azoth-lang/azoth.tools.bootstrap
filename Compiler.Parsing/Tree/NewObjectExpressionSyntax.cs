using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class NewObjectExpressionSyntax : ExpressionSyntax, INewObjectExpressionSyntax
    {
        /// <summary>
        /// Note that this could represent a named or unnamed constructor. So
        /// for an unnamed constructor, it is really the type name. Conceptually
        /// though, the type name is the name of the unnamed constructor. Thus,
        /// this expression's type could be either an object type, or member type.
        /// </summary>
        public ITypeNameSyntax Type { get; }
        public Name? ConstructorName { get; }
        public TextSpan? ConstructorNameSpan { get; }
        public FixedList<IArgumentSyntax> Arguments { get; }
        public Promise<ConstructorSymbol?> ReferencedSymbol { get; } = new Promise<ConstructorSymbol?>();

        public NewObjectExpressionSyntax(
            TextSpan span,
            ITypeNameSyntax typeSyntax,
            Name? constructorName,
            TextSpan? constructorNameSpan,
            FixedList<IArgumentSyntax> arguments)
            : base(span, ExpressionSemantics.Acquire)
        {
            Type = typeSyntax;
            Arguments = arguments;
            ConstructorName = constructorName;
            ConstructorNameSpan = constructorNameSpan;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            var name = ConstructorName != null ? "."+ConstructorName : "";
            return $"new {Type}{name}({string.Join(", ", Arguments)})";
        }
    }
}
