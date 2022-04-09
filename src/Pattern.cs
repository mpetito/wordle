using System.Runtime.CompilerServices;

namespace Wordle;

public class Pattern
{
    private readonly int[] _exact;
    private readonly int[] _partial;

    public Pattern(string value)
    {
        Value = value;

        _exact = value.Select((c, i) => c == 'G' ? i : -1).Where(i => i >= 0).ToArray();
        _partial = value.Select((c, i) => c != 'G' ? i : -1).Where(i => i >= 0).ToArray();
    }

    public string Value { get; init; }

    public const int Length = 5;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool IsMatch(long answer, long guess)
    {
        for (int i = 0; i < _exact.Length; i++)
        {
            var index = _exact[i];
            if (!TryExactMatch(ref answer, guess, index)) return false;
        }

        for (int i = 0; i < _partial.Length; i++)
        {
            var index = _partial[i];
            if (Value[index] == 'Y' != TryPartialMatch(ref answer, guess, index)) return false;
        }

        return true;
    }

    public static Pattern Create(long answer, long guess)
    {
        var pat = new char[Length];
        for (int i = 0; i < Length; i++)
        {
            if (TryExactMatch(ref answer, guess, i)) pat[i] = 'G';
        }

        for (int i = 0; i < Length; i++)
        {
            if (pat[i] == 'G') continue;

            var partial = TryPartialMatch(ref answer, guess, i);

            pat[i] = partial ? 'Y' : 'B';
        }

        return new(new string(pat));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static bool TryExactMatch(ref long answer, long guess, int index)
    {
        var a = Get(answer, index);
        var g = Get(guess, index);

        if (a != g) return false; // attempt partial match

        Del(ref answer, index);
        return true;    // 'G'
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static bool TryPartialMatch(ref long answer, long guess, int index)
    {
        var g = Get(guess, index);

        var i = Find(answer, g);
        if (i < 0) return false;    // 'B'

        Del(ref answer, i);
        return true;    // 'Y'
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static long Get(long word, int index)
    {
        var offset = index * WordList.LetterBits;
        var mask = WordList.LetterMask << offset;
        return (word & mask) >> offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int Find(long word, long c)
    {
        if ((word & (1L << ((int)c + WordList.MaskOffset))) == 0) return -1;

        for (int i = 0; i < WordList.WordLength; i++)
        {
            if (Get(word, i) == c) return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Del(ref long word, int index)
    {
        var offset = index * WordList.LetterBits;
        var mask = WordList.LetterMask << offset;

        word &= ~mask;
    }
}