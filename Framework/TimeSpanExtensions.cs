using System;

namespace Azoth.Tools.Bootstrap.Framework;

public static class TimeSpanExtensions
{
    public static string ToTotalSecondsAndMilliseconds(this TimeSpan timeSpan)
    {
        var totalSeconds = (int)timeSpan.TotalSeconds;
        var milliseconds = timeSpan.Milliseconds;
        if (totalSeconds > 0)
        {
            if (milliseconds > 0) return $"{totalSeconds} s {milliseconds} ms";
            return $"{totalSeconds} s";
        }

        return $"{milliseconds} ms";
    }
}
