using Wordle;
using static Wordle.Session;

var last = -1;
var progress = new Progress<(int Index, int Count, long Word)>(p => 
{
    var (index, count, word) = p;
    if (index < last) return;
    lock(Console.Out)
    {
        if (index > last) 
        {
            last = index;
            Console.SetCursorPosition(0, Console.CursorTop-1);
            Console.WriteLine($"{index:0000}\t{WordList.FromLong(word)}\t{index / (double)count,5:P0}");
        }
    }
});

new SessionAnalyzer(
    // Known("GGGBB", excluded: "", last: "COMFY"),

    Anon("BBYBY YBBBG BBYBG BGBBG BGBGG"),
    Anon("YBBBB BYBYB BGGYB BGGYB"),
    Anon("BBYYB BYYBB YYBYB GGBBG"),
    Anon("GBYBB BGBBB GGBBB GGGBB GGGBB")
).GetPossibleAnswers(progress);
