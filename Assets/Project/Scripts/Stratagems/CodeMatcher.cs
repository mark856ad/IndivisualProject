using System.Collections.Generic;
using UnityEngine;

public enum MatchResult { NoMatch, Partial, Complete }

public class CodeMatcher
{
    private readonly List<StratagemData> candidates;
    private readonly List<Direction> current = new List<Direction>();

    public IReadOnlyList<Direction> Current => current;
    public StratagemData Matched { get; private set; }

    public CodeMatcher(IEnumerable<StratagemData> stratagems)
    {
        candidates = new List<StratagemData>(stratagems);
        ValidateNoPrefixConflict();
    }

    public MatchResult Push(Direction direction)
    {
        Matched = null;
        current.Add(direction);

        bool anyPartial = false;

        foreach (StratagemData s in candidates)
        {
            if (!IsPrefix(current, s.code)) continue;

            if (s.code.Length == current.Count)
            {
                Matched = s;
                current.Clear();
                return MatchResult.Complete;
            }

            anyPartial = true;
        }

        if (anyPartial) return MatchResult.Partial;

        current.Clear();
        return MatchResult.NoMatch;
    }

    public void Reset()
    {
        current.Clear();
        Matched = null;
    }

    private static bool IsPrefix(IReadOnlyList<Direction> prefix, Direction[] code)
    {
        if (prefix.Count > code.Length) return false;

        for (int i = 0; i < prefix.Count; i++)
        {
            if (prefix[i] != code[i]) return false;
        }
        return true;
    }

    private void ValidateNoPrefixConflict()
    {
        for (int a = 0; a < candidates.Count; a++)
        {
            for (int b = 0; b < candidates.Count; b++)
            {
                if (a == b) continue;

                if (IsPrefix(candidates[a].code, candidates[b].code))
                {
                    Debug.LogError($"코드 충돌: '{candidates[a].stratagemName}'의 코드가 '{candidates[b].stratagemName}'의 앞부분과 같습니다.");
                }
            }
        }
    }
}