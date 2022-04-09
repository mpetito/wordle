namespace Wordle;

public class SessionAnalyzer
{
    public static WordList WordList { get; } = new WordList();

    private readonly ISession[] _sessions;

    public SessionAnalyzer(params ISession[] sessions)
    {
        _sessions = sessions;
    }

    public void GetPossibleAnswers(IProgress<(int Index, int Count, long Word)> progress)
    {
        var answers = new HashSet<long>(WordList.Answers);

        foreach (var session in _sessions)
        {
            Console.WriteLine($"Working on {answers.Count:N0} answers...");

            answers.IntersectWith(session.GetAnswers(answers));
        }

        Console.WriteLine($"Found {answers.Count:N0} answers.");
        Console.WriteLine();
        Console.CursorVisible = false;

        var options = answers
            .AsParallel()
            .AsUnordered()
            .Select(guess =>
            {
                var patterns = answers.Select(a => Pattern.Create(a, guess)).DistinctBy(p => p.Value).ToList();
                var score = patterns.Max(pattern => Session.Guess(pattern, guess).GetAnswers(answers).Count());
                return (Guess: guess, Patterns: patterns.Count, Score: score);
            })
            .WithProgressReporting(answers.Count, progress, (o, index, count) => (index, count, o.Guess))
            .OrderBy(o => o.Score)
            .ThenBy(o => o.Patterns)
            .Take(10)
            .ToList();

        Console.CursorVisible = true;

        foreach (var (guess, patterns, score) in options)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(WordList.FromLong(guess));
            Console.ResetColor();

            Console.WriteLine($" would reveal {patterns} patterns with score {score:0}.");
        }
    }
}
