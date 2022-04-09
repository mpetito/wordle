using System.Collections;

namespace Wordle
{
    public class WordList : IEnumerable<long>
    {
        public const int WordLength = 5;

        public WordList()
            : this(GetWords("answers.txt"), GetWords("valid.txt"))
        {
        }

        public WordList(IEnumerable<string> answers, IEnumerable<string> guesses)
        {
            Answers = new List<long>(answers.Select(w => ToLong(w, 1)));
            Guesses = new List<long>(guesses.Select(w => ToLong(w, 0)));
        }

        public List<long> Answers { get; }
        public List<long> Guesses { get; }

        private static IEnumerable<string> GetWords(string file)
        {
            return File.ReadAllLines(file);
        }

        public IEnumerator<long> GetEnumerator() => Answers.Concat(Guesses).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public const int LetterBits = 5;
        public const int LetterMask = 0x1F;
        public const int MaskOffset = 32;
        public const int ScoreOffset = 59;

        public static long ToLong(string word, int score = 0)
        {
            long l = score << ScoreOffset;

            for (int i = 0; i < WordLength; i++)
            {
                char c = word[i];
                l |= (CharToMask(c) << MaskOffset) | (CharToDigit(c) << (LetterBits * i));
            }

            return l;
        }

        public static string FromLong(long value)
        {
            char[] word = new char[WordLength];
            for (int i = 0; i < WordLength; i++)
            {
                word[i] = DigitToChar((uint)(value & LetterMask));
                value >>= LetterBits;
            }

            return new string(word);
        }

        public static bool Contains(long word, char c)
        {
            return (word & (CharToMask(c) << MaskOffset)) != 0;
        }

        public static int GetScore(long word)
        {
            return (int)word >> ScoreOffset;
        }

        private static long CharToDigit(char c) => 1 + c - 'a';
        private static long CharToMask(char c) => 1 << (int)CharToDigit(c);
        private static char DigitToChar(long c) => (char)('a' + c - 1);
    }
}
