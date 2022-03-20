namespace Wordle
{
    public class WordSequence
    {
        public long Word { get; }
        public WordSequence? Next { get; }

        public WordSequence(long word, WordSequence? next = null)
        {
            Word = word;
            Next = next;
        }

        public long Answer => Next?.Answer ?? Word;

        public int Score => WordList.GetScore(Word) + (Next?.Score ?? 0);

        public bool Has(long guess) => Word == guess || (Next?.Has(guess) ?? false);

        public WordSequence Guess(long guess) => new(guess, this);

        public override string ToString()
        {
            if (Next is null) return WordList.FromLong(Word);

            return $"{WordList.FromLong(Word)} {Next}";
        }

        public class EqualityComparer : IEqualityComparer<WordSequence>
        {
            public bool Equals(WordSequence? x, WordSequence? y)
            {
                while (x != null && y != null)
                {
                    if (x.Word != y.Word) return false;

                    x = x.Next;
                    y = y.Next;
                }

                return x == y;
            }

            public int GetHashCode(WordSequence? obj)
            {
                unchecked
                {
                    int hashcode = 1430287;
                    while (obj != null)
                    {
                        hashcode = hashcode * 7302013 ^ (int)obj.Word;
                        obj = obj.Next;
                    }
                    return hashcode;
                }
            }
        }
    }
}
