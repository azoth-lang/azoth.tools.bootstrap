using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Code;

/// The text of a source code file
public sealed class CodeText
{
    public string Text { get; }

    public int Length => Text.Length;

    public TextLines Lines => lines.Value;

    private readonly Lazy<TextLines> lines;

    public string this[TextSpan span] => Text.Substring(span.Start, span.Length);
    public char this[int index] => Text[index];

    public CodeText(string text)
    {
        Text = text;
        lines = new Lazy<TextLines>(GetLines);
    }

    private TextLines GetLines() => new(Text);

    public TextPosition PositionOfStart(in TextSpan span) => PositionOf(span.Start);

    public TextPosition PositionOfEnd(in TextSpan span)
    {
        if (span.IsEmpty)
            return PositionOfStart(span);
        // End is one past, we want the actual last char
        return PositionOf(span.End - 1);
    }

    private TextPosition PositionOf(int charOffset)
    {
        var lineIndex = Lines.LineIndexContainingOffset(charOffset);
        var lineStart = Lines.StartOfLine[lineIndex];

        // TODO handle Unicode
        var column = charOffset - lineStart + 1; // column is one based
        // Account for tabs being multiple columns
        // TODO switch to a for loop when we have range expressions
        var i = lineStart;
        while (i < charOffset)
        {
            if (Text[i] == '\t')
                column += 3;  // tabs are 4 columns, but the character was already counted as 1
            i += 1;
        }

        // The line number is one based while the variable was zero based
        return new TextPosition(charOffset, lineIndex + 1, column);
    }
}
