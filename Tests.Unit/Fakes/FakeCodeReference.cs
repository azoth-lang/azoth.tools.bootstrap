using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Fakes;

internal class FakeCodeReference : CodeReference
{
    #region Singleton
    public static readonly FakeCodeReference Instance = new();

    private FakeCodeReference()
        : base(FixedList.Empty<string>(), isTesting: false)
    { }
    #endregion

    public override string ToString()
        => throw new InvalidOperationException("Fake doesn't support ToString()");
}
