namespace Wordle;

public interface ISession
{
    ParallelQuery<long> GetAnswers(IEnumerable<long> answers);
    ParallelQuery<WordSequence> GetSequences(IEnumerable<long> answers);
}

public static class Session
{
    public static ISession Anon(string sequence)
    {
        return new AnonymousSession(GetSteps(sequence));
    }

    public static ISession Known(string sequence, string excluded, string last)
    {
        return new KnownSession(excluded.ToLower(), WordList.ToLong(last.ToLower()), GetSteps(sequence));
    }

    public static ISession Guess(Pattern pattern, long guess)
    {
        return new KnownSession(string.Empty, guess, pattern.Value);
    }

    private static string[] GetSteps(string sequence) => sequence.Split(' ');

    private class KnownSession : AnonymousSession
    {
        public string Excluded { get; }
        public long Last { get; }

        public KnownSession(string excluded, long last, params string[] steps)
            : base(steps)
        {
            Excluded = excluded;
            Last = last;
        }

        public override ParallelQuery<long> GetAnswers(IEnumerable<long> answers)
        {
            return base.GetAnswers(Filter(answers));
        }

        public override ParallelQuery<WordSequence> GetSequences(IEnumerable<long> answers)
        {
            return base.GetSequences(Filter(answers));
        }

        private IEnumerable<long> Filter(IEnumerable<long> answers)
        {
            var result = answers;

            if (Excluded.Length > 0)
            {
                result = result.Where(answer => !Excluded.Any(c => WordList.Contains(answer, c)));
            }

            return result.Where(answer => Steps.Last().IsMatch(answer, Last));
        }
    }

    private class AnonymousSession : ISession
    {
        protected Pattern[] Steps { get; }

        public AnonymousSession(params string[] steps)
        {
            Steps = steps.Select(step => new Pattern(step)).ToArray();
        }

        public virtual ParallelQuery<long> GetAnswers(IEnumerable<long> answers)
        {
            return answers
                .Select(answer => new WordSequence(answer))
                .AsParallel()
                .AsUnordered()
                .Where(seq => GetSequences(seq, Steps.Length - 1).Any())
                .Select(seq => seq.Answer);
        }

        public virtual ParallelQuery<WordSequence> GetSequences(IEnumerable<long> answers)
        {
            return answers
                .Select(answer => new WordSequence(answer))
                .AsParallel()
                .AsUnordered()
                .SelectMany(seq => GetSequences(seq, Steps.Length - 1));
        }

        private IEnumerable<WordSequence> GetSequences(WordSequence sequence, int stepIndex)
        {
            var step = Steps[stepIndex];
            var answer = sequence.Answer;

            var guesses = SessionAnalyzer.WordList
                .Where(word => !sequence.Has(word) && step.IsMatch(answer, word))
                .Select(word => sequence.Guess(word));

            if (stepIndex == 0) return guesses;

            return guesses.SelectMany(next => GetSequences(next, stepIndex - 1));
        }
    }
}