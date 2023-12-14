using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IEqualsToken),
    typeof(IPlusEqualsToken),
    typeof(IMinusEqualsToken),
    typeof(IAsteriskEqualsToken),
    typeof(ISlashEqualsToken))]
public interface IAssignmentOperatorToken : IOperatorToken { }

public partial interface IEqualsToken : IAssignmentOperatorToken { }
public partial interface IPlusEqualsToken : IAssignmentOperatorToken { }
public partial interface IMinusEqualsToken : IAssignmentOperatorToken { }
public partial interface IAsteriskEqualsToken : IAssignmentOperatorToken { }
public partial interface ISlashEqualsToken : IAssignmentOperatorToken { }
