using Xunit;

namespace Wordle.Tests
{
    public class PatternTests
    {
        [InlineData("BBBBB", "abcde", "zyxwv")]
        [InlineData("GGGGG", "abcde", "abcde")]
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

        [InlineData("GBBBB", "abcde", "aaaaa")]
        [InlineData("GBBBG", "abcda", "aaaaa")]
        [InlineData("BBYBB", "abcda", "xxaxx")]
        [InlineData("GBYBB", "abcda", "axaxx")]
        [InlineData("BYYBB", "abcda", "xaaax")]
        [InlineData("BYYBB", "bcdaa", "xaaxx")]
        [InlineData("BBBYG", "abcda", "xxxaa")]
        [InlineData("BBBBG", "qbcda", "xxxaa")]
        [InlineData("GGGGG", "aaaaa", "aaaaa")]
        [Theory]
        public void RepeatedLetterPatternMatches(string patternValue, string answerWord, string guessWord)
        {
            var pattern = new Pattern(patternValue);

            var answer = WordList.ToLong(answerWord);
            var guess = WordList.ToLong(guessWord);

            Assert.Equal(patternValue, Pattern.Create(answer, guess).Value);
            Assert.True(pattern.IsMatch(answer, guess));
        }
    }
}
