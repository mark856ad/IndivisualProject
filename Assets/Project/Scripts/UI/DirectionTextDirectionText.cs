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

    private static string ToArrow(Direction d)
    {
        return d == Direction.Up ? "↑"
            : d == Direction.Down ? "↓"
            : d == Direction.Left ? "←"
            : "→";
    }
}