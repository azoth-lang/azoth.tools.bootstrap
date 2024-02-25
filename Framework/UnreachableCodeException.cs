using System;

namespace Azoth.Tools.Bootstrap.Framework;

public class UnreachableCodeException : Exception
{
    public UnreachableCodeException()
    {
    }

    public UnreachableCodeException(string message)
        : base(message)
    {

    }
}
