namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

public static class SharingIsTrackedExtensions
{
    public static bool SharingIsTracked(this IBindingNode node)
    {
        // Any lent parameter needs tracked to prevent sharing with it
        if (node.IsLentBinding) return true;
        return node.BindingType.SharingIsTracked();
    }
}
