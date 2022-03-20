namespace Wordle
{
    public record Pattern(string Value)
    {
        public const int Length = 5;

        public bool IsMatch(long answer, long guess)
        {
            for (int i = 0; i < Length; i++)
            {
                if (WordList.GetMatch(answer, guess, i) != Value[i]) return false;
            }

            return true;
        }
    }
}
