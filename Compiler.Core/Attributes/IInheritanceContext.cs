namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface IInheritanceContext
{
    /// <summary>
    /// Mark that any attributes based on this context may not be final and cannot be cached.
    /// </summary>
    /// <remarks>Any time that the parent is accessed from a non-final child, it is possible that a
    /// rewrite will later change the parent. Thus, any attribute that depends directly or
    /// indirectly on it should not be cached. This method needs to be called in that case to
    /// prevent the attribute caching.</remarks>
    void MarkNonFinal();
}
