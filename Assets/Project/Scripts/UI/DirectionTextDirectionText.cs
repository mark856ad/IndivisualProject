public static class DirectionText
{
    public static string ToArrows(Direction[] code)
    {
        string text = "";
        foreach (Direction d in code)
        {
            text += ToArrow(d);
        }
        return text;
    }

    public static string ToArrowsColored(Direction[] code, int matchedCount)
    {
        string text = "";
        for (int i = 0; i < code.Length; i++)
        {
            string arrow = ToArrow(code[i]);
            text += i < matchedCount ? $"<color=#66FF66>{arrow}</color>" : arrow;
        }
        return text;
    }
    public static int MatchedPrefixCount(System.Collections.Generic.IReadOnlyList<Direction> input, Direction[] code)
    {
        if (input.Count == 0 || input.Count > code.Length) return 0;

        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] != code[i]) return 0;
        }

        return input.Count;
    }
    

    private static string ToArrow(Direction d)
    {
        return d == Direction.Up ? "↑"
            : d == Direction.Down ? "↓"
            : d == Direction.Left ? "←"
            : "→";
    }
}