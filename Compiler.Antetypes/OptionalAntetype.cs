namespace Compiler.Antetypes;

public sealed class OptionalAntetype : INonVoidAntetype
{
    public INonVoidAntetype Referent { get; }

    public OptionalAntetype(INonVoidAntetype referent)
    {
        Referent = referent;
    }
}
