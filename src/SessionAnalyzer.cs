namespace Wordle
{
    internal class SessionAnalyzer
    {
        public static WordList WordList { get; } = new WordList();

        private readonly Session[] _sessions;

        public SessionAnalyzer(params Session[] sessions)
        {
            _sessions = sessions;
        }

        public IEnumerable<WordSequence> GetPossibleAnswers()
        {
            var answers = new HashSet<long>(_sessions.First().GetAnswers(WordList.Answers));

            foreach (var session in _sessions.Skip(1))
            {
                Console.WriteLine($"Working on {answers.Count:N0} answers...");

                answers.IntersectWith(session.GetAnswers(answers));
            }

            Console.WriteLine($"Found {answers.Count:N0} answers.");

            var sequences = _sessions
                .SelectMany(session => session.GetSequences(answers))
                .Count();

            Console.WriteLine($"Found {sequences:N0} sequences.");

            return new WordSequence[0];
        }

        public class Session
        {
            private Pattern[] _steps;

            public Session(params string[] steps)
            {
                _steps = steps.Select(step => new Pattern(step)).ToArray();
            }

            public ParallelQuery<long> GetAnswers(IEnumerable<long> answers)
            {
                return answers
                    .Select(answer => new WordSequence(answer))
                    .AsParallel()
                    .AsUnordered()
                    .Where(seq => GetSequences(seq, _steps.Length - 1).Any())
                    .Select(seq => seq.Answer);
            }

            public ParallelQuery<WordSequence> GetSequences(IEnumerable<long> answers)
            {
                return answers
                    .Select(answer => new WordSequence(answer))
                    .AsParallel()
                    .AsUnordered()
                    .SelectMany(seq => GetSequences(seq, _steps.Length - 1));
            }

            private IEnumerable<WordSequence> GetSequences(WordSequence sequence, int stepIndex)
            {
                var step = _steps[stepIndex];
                var answer = sequence.Answer;

                var guesses = WordList
                    .Where(word => !sequence.Has(word) && step.IsMatch(answer, word))
                    .Select(word => sequence.Guess(word));

                if (stepIndex == 0) return guesses;

                return guesses.SelectMany(next => GetSequences(next, stepIndex - 1));
            }
        }
    }
}
