namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IDefinitionSyntax
{
    /// <summary>
    /// The span of whatever would count as the "name" of this declaration
    /// for things like operator overloads, constructors and destructors,
    /// this won't be just an identifier. For example, it could be:
    /// * "operator +"
    /// * "new foo"
    /// * "delete"
    /// </summary>
    // TODO when partial properties are supported, add this to put doc comment on the property
    //TextSpan NameSpan { get; }
}
