using Xunit;

namespace Wordle.Tests
{
    public class PatternTests
    {
        [InlineData("BBBBB", "abcde", "zyxwv")]
        [InlineData("GGGGG", "abcde", "abcde")]
        [InlineData("GBBBB", "abcde", "aaaaa")]
        [InlineData("YYGYY", "abcde", "baced")]
        [InlineData("YYBYY", "abcde", "baxed")]
        [Theory]
        public void PositivePatternMatches(string patternValue, string answerWord, string guessWord)
        {
            var pattern = new Pattern(patternValue);

            var answer = WordList.ToLong(answerWord);
            var guess = WordList.ToLong(guessWord);

            Assert.True(pattern.IsMatch(answer, guess));
        }
    }
}
